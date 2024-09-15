using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using NodaTime.TimeZones;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Chameleon.Core.Extensions;

namespace Chameleon.ThirdParty.GeoIp
{
    public class GeoIpApi
    {
        // Make singleton
        public static GeoIpApi Instance { get; } = new GeoIpApi();

        public Task<string> GetIPApi(string proxyUrl, Action<string> onretry, string proxyUsername = null, string proxyPassword = null)
            => GetHttpResponseContent(proxyUrl, "http://ip-api.com/json", onretry, proxyUsername, proxyPassword);

        private async Task<string> GetHttpResponseContent(string proxyUrl, string requestUri, Action<string> onretry, string proxyUsername = null, string proxyPassword = null)
        {
            HttpClientHandler handler = await InitializeHttpClientHandlerWithRetry(proxyUrl, proxyUsername, proxyPassword, onretry);
            HttpClient client = new(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            try
            {
                HttpResponseMessage response = await Policy.WrapAsync(
                    Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode >= HttpStatusCode.InternalServerError).Or<HttpRequestException>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt), (outcome, timespan, retryAttempt, context) =>
                        {
                            onretry($"Timezone Request from proxy failed. Retry {retryAttempt} for {context.PolicyKey} at {context.OperationKey}: due to {outcome.Exception?.Message} {outcome.Result?.StatusCode}");
                        }),
                    Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).Or<HttpRequestException>()
                        .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: 4,
                            durationOfBreak: TimeSpan.FromSeconds(3)
                        )).ExecuteAsync(() => client.GetAsync(requestUri));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
                }
            }
            finally
            {
                client.Dispose();
            }
        }

        private async Task<HttpClientHandler> InitializeHttpClientHandlerWithRetry(string proxyUrl, string proxyUsername, string proxyPassword, Action<string> onretry)
        {
            return await Policy.Handle<WebException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt), (exception, timespan, retryAttempt, context) =>
                {
                    onretry($"Proxy initialization failed. Retry {retryAttempt}: due to {exception.Message}");
                })
                .ExecuteAsync(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(proxyUrl)
                    };
                    if (proxyUsername.HasAny() && proxyPassword.HasAny())
                    {
                        handler.Proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                    }
                    return Task.FromResult(handler);
                });
        }
    }
}
