using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsApi
        : ApiLayer<UserDefaultSettingsDto, int, CreateUserDefaultSettingsDto, UserDefaultSettingsDto>
        , IUserDefaultSettingsApi
    {
        public UserDefaultSettingsApi(
            IApiClient apiClient
            ) : base(apiClient, "userDefaultSettings")
        {
        }
    }
}
