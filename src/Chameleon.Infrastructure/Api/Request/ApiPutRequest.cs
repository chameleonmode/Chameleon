using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class ApiPutRequest : ApiRequestWithBody<ApiPutRequest>
    {
        public ApiPutRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            ) : base(session, configuration, "PUT")
        {
        }
    }
}
