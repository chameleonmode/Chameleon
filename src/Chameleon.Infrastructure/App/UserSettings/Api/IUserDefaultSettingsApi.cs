using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserSettings
{
    public interface IUserDefaultSettingsApi
        : IApiLayer<UserDefaultSettingsDto
            , int
            , CreateUserDefaultSettingsDto
            , UserDefaultSettingsDto>
        , ISingletonDependency
    {
    }
}
