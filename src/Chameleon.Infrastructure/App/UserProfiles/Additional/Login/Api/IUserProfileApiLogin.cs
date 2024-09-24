using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public interface IUserProfileApiLogin
        : IApiLayer<UserProfileLoginDto, int, CreateUserProfileLoginDto, UserProfileLoginDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
