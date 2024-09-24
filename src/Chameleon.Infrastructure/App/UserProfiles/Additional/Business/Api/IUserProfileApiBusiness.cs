using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public interface IUserProfileApiBusiness
        : IApiLayer<UserProfileBusinessDto, int, CreateUserProfileBusinessDto, UserProfileBusinessDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
