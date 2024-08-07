using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Authentication;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text.Json;
using System.Net.Http;
using Chameleon.Core.Util;
using Polly;

namespace Chameleon.Infrastructure.Api
{
    public abstract class ApiRequest<TApiRequest>(IAuthSession session, IApplicationConfiguration configuration)
        where TApiRequest : ApiRequest<TApiRequest>
    {
        private HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
                             
        private HttpRequestMessage _request;

        private string _requestUrl;
        private string _requestQuery;
        private string _responseBody;
        private readonly int _retryCount = 3;


        public TApiRequest ForUrl(string requestUrl)
        {
            _requestUrl = GetUrl(requestUrl);
            return (TApiRequest)this;
        }

        public TApiRequest WithRetryCount(int retryCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);

            //_retryCount = retryCount;
            return (TApiRequest)this;
        }

        public TApiRequest Send()
        {
            try
            {
                CreateRequest();
                SetAuthHeader();
                InitializeRequest(_request);
                ReadResponse();
                return (TApiRequest)this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

            //Exception? err = null;
            //TaskUtil.AwaitFor(() =>
            //{
            //    CreateRequest();
            //    SetAuthHeader();
            //    InitializeRequest(_request);
            //    ReadResponse();
            //    return (TApiRequest)this != null;
            //}, _retryCount, 500, (e) => 
            //{ 
            //        err = e;
            //}).Wait();

            //if(err != null)
            //    throw new InvalidOperationException(err.Message, err);

            //return (TApiRequest)this;
        }

        public void GetResult()
        {
            if (string.IsNullOrEmpty(_responseBody))
            {
                return;
            }

            if (_responseBody.Contains("SQLSTATE"))
            {
                throw new InvalidOperationException(_responseBody);
            }
        }

        private void CreateRequest()
        {
            var requestUrl = GetRequestUrl();
            // Configure the request
            _request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        }

        public TApiRequest WithQuery(object query)
        {
            if (query == null)
            {
                _requestQuery = null;
                return (TApiRequest)this;
            }

            var properties = query
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var builder = new StringBuilder();
            foreach (var property in properties)
            {
                var value = property.GetValue(query, null);
                if (value == null)
                {
                    continue;
                }

                var valueType = value.GetType();
                var defaultValue = Activator.CreateInstance(valueType);
                if (Equals(defaultValue, value))
                {
                    continue;
                }

                var queryValue = WebUtility.UrlEncode(value.ToString());
                if (builder.Length != 0)
                {
                    builder.Append('&');
                }
                builder.Append($"{property.Name}={queryValue}");
            }

            if (builder.Length == 0)
            {
                return (TApiRequest)this;
            }

            _requestQuery = builder.ToString();
            return (TApiRequest)this;
        }

        public string GetRequestUrl()
        {            
            if (_requestQuery == null)
            {
                return _requestUrl;
            }
            return $"{_requestUrl}?{_requestQuery}";
        }

        private void ReadResponse()
        {
            try
            {
                // You can send the request and get the response synchronously if needed
                //using var httpResponse = _httpClient.SendAsync(_request).GetAwaiter().GetResult();
               using HttpResponseMessage httpResponse = Policy.WrapAsync(
                   Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                   .Or<HttpRequestException>()
                   .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))),
                   //.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (outcome, timespan, retryAttempt, context) =>
                   //{
                   //     //onretry($"Timezone Request from proxy failed. Retry {retryAttempt} for {context.PolicyKey} at {context.OperationKey}: due to {outcome.Exception?.Message} {outcome.Result?.StatusCode}");
                   //}),
                   Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                   .Or<HttpRequestException>()
                   .CircuitBreakerAsync(
                       handledEventsAllowedBeforeBreaking: _retryCount,
                       durationOfBreak: TimeSpan.FromSeconds(30)
                   )).ExecuteAsync(() => _httpClient.SendAsync(_request)).GetAwaiter().GetResult();

                // Process the response if needed (e.g., check status code, read content)
                if (httpResponse.IsSuccessStatusCode)
                    _responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                else
                {
                    // Log the error details
                    var errorContent = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _responseBody = errorContent;
                    throw new InvalidOperationException($"Request failed with status code {httpResponse.StatusCode} and reason phrase '{httpResponse.ReasonPhrase}'. Content: {errorContent}");
                }
            }
            catch (WebException ex)
            {
                HandleResponseException(ex);
                throw;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        private void HandleResponseException(WebException ex)
        {
            if(ex.Response is not HttpWebResponse httpResponse)
                throw new InvalidOperationException(ex.Message, ex);

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                throw new AuthenticationException("Unauthorized", ex);

            if (JsonSerializer.Deserialize<ApiResponseDto>(_responseBody) is not ApiResponseDto responseDto)
                return;

            if (responseDto?.Error is not ApiResponseErrorDto responseError)
                return;

            if (responseError.Code == (int)HttpStatusCode.Unauthorized)
                throw new AuthenticationException("Unauthorized", ex);

            throw new InvalidOperationException(ex.Message, ex);
        }

        protected virtual string BaseUrl => configuration.ApiBaseUrl;

        public virtual TResult GetResult<TResult>()
        {
            return new ApiResult<TResult>(_responseBody)
                .Deserialize();
        }

        protected virtual string GetUrl(string url)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));

            if (url[0] != '/')
            {
                url = '/' + url;
            }            

            var baseUrl = BaseUrl;
            if (baseUrl.EndsWith('/'))
            {
                baseUrl = baseUrl[..^1];
            }
            return baseUrl + url;
        }
        
        protected virtual void SetAuthHeader()
        {
            if (session.HasAuthToken)
            {
                _request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", session.AuthToken);
            }
        }

        protected virtual void InitializeRequest(HttpRequestMessage request)
        {
        }
    }
}
