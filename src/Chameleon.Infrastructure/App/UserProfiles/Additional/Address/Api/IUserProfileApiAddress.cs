using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public interface IUserProfileApiAddress
        : IApiLayer<UserProfileAddressDto, int, CreateUserProfileAddressDto, UserProfileAddressDto>
        , ISingletonDependency
    {
    }
}
