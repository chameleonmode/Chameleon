using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public class UserProfileApiPerson 
        : ApiLayer<UserProfilePersonDto, int, CreateUserProfilePersonDto, UserProfilePersonDto>
        , IUserProfileApiPerson
    {
        public UserProfileApiPerson(IApiClient apiClient) 
            : base(apiClient, "person") 
        { }
    }
}
