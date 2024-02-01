using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class ApiPostRequest : ApiRequestWithBody<ApiPostRequest>
    {
        public ApiPostRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            ) : base(session, configuration, "POST")
        {
        }
    }
}
