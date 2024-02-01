using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserSettings
{
    public interface IUserSettingsApi
        : IApiLayer<UserSettingsDto
            , int
            , UserSettingsDto>
        , ISingletonDependency
    {
    }
}
