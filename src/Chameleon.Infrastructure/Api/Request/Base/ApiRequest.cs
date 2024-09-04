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
using Chameleon.Interfaces.Dialogs;
using Chameleon.Common.Helpers;
using Polly.CircuitBreaker;
using Chameleon.Auth.Api;
using System.Linq;
using System.Text.RegularExpressions;

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

        // You can send the request and get the response synchronously if needed
        private void ReadResponse()
        {
            try
            {
                var retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (outcome, timespan, retryAttempt, context) =>
                    {
                        ToasterHelper.ShowErr($"Request Failed: Retry {retryAttempt}: {outcome.Result?.StatusCode}");
                    });

                var refreshPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), async (outcome, timespan, retryAttempt, context) =>
                    {
                        ToasterHelper.ShowErr($"Request Failed: Retry {retryAttempt}: {outcome.Result?.StatusCode}");

                        var refresh = await AuthApiClient.Instance.RefreshTokenAsync(session.AuthToken, session.AuthRefreshToken, 0) ?? throw new UnauthorizedAccessException("Refresh token failed");
                        session.AuthToken = refresh.NewAccessToken;
                        session.AuthRefreshToken = refresh.NewRefreshToken;
                        session.ExpireInSeconds = refresh.ExpireInSeconds;
                    });

                var circuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<HttpRequestException>()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: _retryCount,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, breakDelay) =>
                        {
                            _responseBody = outcome.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            // Log the circuit breaker opening
                            Console.WriteLine($"Circuit breaker opened due to: {outcome.Exception?.Message ?? outcome.Result.ReasonPhrase}");
                        },
                        onReset: () =>
                        {
                            // Log the circuit breaker resetting
                            Console.WriteLine("Circuit breaker reset.");
                        },
                        onHalfOpen: () =>
                        {
                            // Log the circuit breaker half-open state
                            Console.WriteLine("Circuit breaker is half-open.");
                        });

                var policyWrap = Policy.WrapAsync(retryPolicy, refreshPolicy, circuitBreakerPolicy);

                using HttpResponseMessage httpResponse = policyWrap.ExecuteAsync(() =>
                {
                    CreateRequest();
                    SetAuthHeader();
                    InitializeRequest(_request);
                    return _httpClient.SendAsync(_request);
                }).GetAwaiter().GetResult();

                _responseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string FormatErrorMessage(HttpStatusCode statusCode, string reasonPhrase, string responseBody)
                    {
                        var errorDetails = new StringBuilder();
                        //errorDetails.AppendLine($"Request failed with status code {statusCode} and reason phrase '{reasonPhrase}'.");
                        errorDetails.AppendLine($"Request failed. ");

                        // Extract error message
                        //var errorMessageMatch = Regex.Match(responseBody, @"""message"":""([^""]+)""");
                        //if (errorMessageMatch.Success)
                        //{
                        //    errorDetails.AppendLine($"Error Message: {errorMessageMatch.Groups[1].Value}");
                        //}

                        // Extract error details
                        var errorDetailsMatch = Regex.Match(responseBody, @"""details"":""([^""]+)""");
                        if (errorDetailsMatch.Success)
                        {
                            var cleanedDetails = errorDetailsMatch.Groups[1].Value.Replace("\\r\\n", " ");
                            errorDetails.AppendLine($"{cleanedDetails}");
                        }

                        // Extract validation errors
                        //var validationErrorsMatch = Regex.Match(responseBody, @"""validationErrors"":\[(.*?)\]");
                        //if (validationErrorsMatch.Success)
                        //{
                        //    errorDetails.AppendLine("Validation Errors:");
                        //    var validationErrors = validationErrorsMatch.Groups[1].Value;
                        //    var validationErrorMatches = Regex.Matches(validationErrors, @"{""message"":""([^""]+)"",""members"":\[""([^""]+)""\]}");
                        //    foreach (Match match in validationErrorMatches)
                        //    {
                        //        errorDetails.AppendLine($" - {match.Groups[1].Value} (Members: {match.Groups[2].Value})");
                        //    }
                        //}

                        return errorDetails.ToString();
                    }
                    var errorMessage = FormatErrorMessage(httpResponse.StatusCode, httpResponse.ReasonPhrase, _responseBody);
                    throw new InvalidOperationException(errorMessage);
                }
            }
            catch (BrokenCircuitException ex)
            {
                // Handle the open circuit scenario
                Console.WriteLine("Circuit is open and not allowing calls.");
                if(JsonSerializer.Deserialize<ApiResponseDto>(_responseBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) is not ApiResponseDto responseDto)
                    throw new InvalidOperationException("Circuit is open and not allowing calls.", ex);
                else
                    throw new InvalidOperationException(responseDto.Error?.Message, ex);
            }
            catch (WebException ex)
            {
                HandleResponseException(ex);
                throw;
            }
            catch (Exception ex)
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
