using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public interface IUserProfileApiPerson
        : IApiLayer<UserProfilePersonDto, int, CreateUserProfilePersonDto, UserProfilePersonDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
