using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserSettings
{
    public interface IUserDefaultSettingsApi
        : IApiLayer<UserDefaultSettingsDto
            , int
            , CreateUserDefaultSettingsDto
            , UserDefaultSettingsDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
