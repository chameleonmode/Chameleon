using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsApi
       : ApiLayer<UserSettingsDto>
       , IUserSettingsApi
    {
        public UserSettingsApi(
            IApiClient apiClient
            ) : base(apiClient, "userSettings")
        {
        }
    }
}
