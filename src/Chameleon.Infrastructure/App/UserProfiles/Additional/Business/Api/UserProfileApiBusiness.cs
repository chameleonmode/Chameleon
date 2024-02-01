using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public class UserProfileApiBusiness
        : ApiLayer<UserProfileBusinessDto, int, CreateUserProfileBusinessDto, UserProfileBusinessDto>
        , IUserProfileApiBusiness
    {
        public UserProfileApiBusiness(IApiClient apiClient) 
            : base(apiClient, "business")
        { }
    }
}
