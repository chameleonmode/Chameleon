using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net;

namespace Chameleon.Infrastructure.Api
{
    public class ApiRequestWithBody<TApiRequest> : ApiRequest<TApiRequest>
        where TApiRequest : ApiRequestWithBody<TApiRequest>
    {
        private object _requestBody;
        private string _requestJson;
        private readonly string _httpMethod;
        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public ApiRequestWithBody(
            IAuthSession session,
            IApplicationConfiguration configuration,
            string httpMethod
            ) : base(session, configuration)
        {
            _httpMethod = httpMethod;
        }

        public TApiRequest WithBody(object requestBody)
        {
            _requestBody = requestBody;
            return (TApiRequest)this;
        }

        protected override void InitializeRequest(HttpWebRequest request)
        {
            request.Method = _httpMethod;
            if (_requestBody == null)
            {
                return;
            }

            request.ContentType = "application/json";
            using (var requestStream = request.GetRequestStream())
            {
                using (var streamWriter = new StreamWriter(requestStream))
                {
                    streamWriter.Write(GetRequestBodyAsJson());
                }
            }
        }

        private string GetRequestBodyAsJson()
        {
            _requestJson = JsonConvert.SerializeObject(_requestBody, _settings);
            return _requestJson;
        }
    }
}
