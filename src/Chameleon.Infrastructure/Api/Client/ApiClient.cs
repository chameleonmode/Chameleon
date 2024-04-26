using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class ApiClient 
        : NativeApiClient
        , IApiClient
    {
        public ApiClient(
            IAuthSession session,
            IApplicationConfiguration configuration,
            IToastNotificationService toaster) 
            : base(session, configuration,toaster)
        {
        }
    }
}
