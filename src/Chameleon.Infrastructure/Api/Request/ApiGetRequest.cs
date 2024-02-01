using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class ApiGetRequest : ApiRequest<ApiGetRequest>
    {
        public ApiGetRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            ) : base(session, configuration)
        {
        }
    }
}
