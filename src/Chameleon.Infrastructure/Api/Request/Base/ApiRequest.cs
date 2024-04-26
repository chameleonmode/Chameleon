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

namespace Chameleon.Infrastructure.Api
{
    public abstract class ApiRequest<TApiRequest>
        where TApiRequest : ApiRequest<TApiRequest>
    {
        protected readonly IAuthSession _session;
        protected readonly IApplicationConfiguration _configuration;

        private string _requestUrl;
        private string _requestQuery;
        private string _responseBody;
        private HttpWebRequest _request;

        public ApiRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            )
        {
            _session = session;
            _configuration = configuration;
        }

        public string GetResponseBody()
        {
            return _responseBody;
        }

        protected HttpWebRequest Request => _request;

        public TApiRequest ForUrl(string requestUrl)
        {
            _requestUrl = GetUrl(requestUrl);
            return (TApiRequest)this;
        }

        private int _retryCount = 3;
        public TApiRequest WithRetryCount(int retryCount)
        {
            if (retryCount <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            _retryCount = retryCount;
            return (TApiRequest)this;
        }

        private int _retryIndex = 0;
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

        public virtual TResult GetResult<TResult>()
        {
            return new ApiResult<TResult>(_responseBody)
                .Deserialize();
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
            _request = (HttpWebRequest)WebRequest.Create(requestUrl);//TODO: update
            _request.AutomaticDecompression = DecompressionMethods.GZip;
            _request.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
        }

        private static bool RemoteCertificateValidationCallback(
            object sender, X509Certificate certificate, X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
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
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                ;

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

        protected virtual void InitializeRequest(HttpWebRequest request)
        {
        }

        private void ReadResponse()
        {
            try
            {
                using (var httpResponse = (HttpWebResponse)_request.GetResponse())
                {
                    _responseBody = ReadResponse(httpResponse);
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

            var responseBody = ReadResponse(httpResponse);
            HandleResponseException(ex, responseBody);
        }

        private void HandleResponseException(WebException ex, string responseBody)
        {
            ApiResponseDto responseDto;
            try
            {
                responseDto = JsonSerializer.Deserialize<ApiResponseDto>(responseBody);
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

        private string ReadResponse(HttpWebResponse httpResponse)
        {
            using (var responseStream = httpResponse.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
            {
                return streamReader
                    .ReadToEnd()
                    .Trim();
            }
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

        protected virtual string BaseUrl 
            => _configuration.ApiBaseUrl;
        
        protected virtual void SetAuthHeader()
        {
            if (_session.HasAuthToken)
            {
                _request.Headers["Authorization"] = $"Bearer {_session.AuthToken}";
            }
        }
    }
}
