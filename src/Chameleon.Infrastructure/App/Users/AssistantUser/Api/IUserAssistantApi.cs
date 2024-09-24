using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Users
{
    public interface IUserAssistantApi
        : IApiLayer<UserAssistantDto
            , long
            , CreateUserAssistantDto
            , UserAssistantDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void AddProfiles(UserAssistantDto input);
        void SetCanCreateProfiles(long assistantId, bool canCreateProfiles);
    }
}
