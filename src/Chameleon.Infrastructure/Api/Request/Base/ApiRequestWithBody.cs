using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Chameleon.Infrastructure.Api
{
    public class ApiRequestWithBody<TApiRequest> : ApiRequest<TApiRequest>
        where TApiRequest : ApiRequestWithBody<TApiRequest>
    {
        private object _requestBody;
        private string _requestJson;
        private readonly HttpMethod _httpMethod;
        private readonly JsonSerializerOptions _settings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiRequestWithBody(
            IAuthSession session,
            IApplicationConfiguration configuration,
            HttpMethod httpMethod
            ) : base(session, configuration)
        {
            _httpMethod = httpMethod;
        }

        public TApiRequest WithBody(object requestBody)
        {
            _requestBody = requestBody;
            return (TApiRequest)this;
        }

        protected override void InitializeRequest(HttpRequestMessage request)
        {
            request.Method = _httpMethod;

            if (_requestBody == null)
                return;

            // Set the Content-Type header
            request.Content = new StringContent(GetRequestBodyAsJson(), Encoding.UTF8, "application/json");
        }

        private string GetRequestBodyAsJson()
        {
            _requestJson = JsonSerializer.Serialize(_requestBody, _settings);
            return _requestJson;
        }
    }
}
