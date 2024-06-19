using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;
using System.Net;
using System.Net.Http;

namespace Chameleon.Infrastructure.Api
{
    public class ApiDeleteRequest : ApiRequest<ApiDeleteRequest>
    {
        public ApiDeleteRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            ) : base(session, configuration)
        {
        }

        protected override void InitializeRequest(HttpRequestMessage request)
        {
            request.Method = HttpMethod.Delete;
        }
    }
}
