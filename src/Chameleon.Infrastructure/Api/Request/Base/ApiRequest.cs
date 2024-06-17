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

namespace Chameleon.Infrastructure.Api
{
    public abstract class ApiRequest<TApiRequest>(IAuthSession session, IApplicationConfiguration configuration)
        where TApiRequest : ApiRequest<TApiRequest>
    {
        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
                             
        private HttpRequestMessage _request;

        private string _requestUrl;
        private string _requestQuery;
        private string _responseBody;
        private int _retryIndex = 0;
        private int _retryCount = 3;


        public TApiRequest ForUrl(string requestUrl)
        {
            _requestUrl = GetUrl(requestUrl);
            return (TApiRequest)this;
        }

        public TApiRequest WithRetryCount(int retryCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryCount);

            _retryCount = retryCount;
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
            catch
            {
                if (++_retryIndex >= _retryCount)
                {
                    throw;
                }
                return Send();
            }
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
                var response = _httpClient.SendAsync(_request).GetAwaiter().GetResult();

                // Process the response if needed (e.g., check status code, read content)
                if (response.IsSuccessStatusCode)
                {
                    _responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (WebException ex)
            {
                HandleResponseException(ex);
                throw;
            }
        }

        private void HandleResponseException(WebException ex)
        {
            var httpResponse = (HttpWebResponse)ex.Response;
            if (httpResponse == null)
            {
                ThrowInvalidOperationException(ex);
            }

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                ThrowUnauthorizedException(ex);
            }

            ApiResponseDto responseDto;
            try
            {
                responseDto = JsonSerializer.Deserialize<ApiResponseDto>(_responseBody);
            }
            catch
            {
                return;
            }

            var responseError = responseDto?.Error;
            if (responseError == null)
            {
                return;
            }

            if (responseError.Code == (int)HttpStatusCode.Unauthorized)
            {
                ThrowUnauthorizedException(ex);
            }

            ThrowInvalidOperationException(ex, responseError.Message);
        }

        private void ThrowUnauthorizedException(Exception ex)
        {
            throw new AuthenticationException("Unauthorized", ex);
        }

        private void ThrowInvalidOperationException(Exception ex, string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = ex.Message;
            }
            throw new InvalidOperationException(message, ex);
        }

        protected virtual string BaseUrl => configuration.ApiBaseUrl;

        public virtual TResult GetResult<TResult>()
        {
            return new ApiResult<TResult>(_responseBody)
                .Deserialize();
        }

        protected virtual string GetUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException();
            }

            if (url[0] != '/')
            {
                url = '/' + url;
            }            

            var baseUrl = BaseUrl;
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
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
