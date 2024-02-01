using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public class UserProfileApiLogin 
        : ApiLayer<UserProfileLoginDto, int, CreateUserProfileLoginDto, UserProfileLoginDto>
        , IUserProfileApiLogin
    {
        public UserProfileApiLogin(IApiClient apiClient) 
            : base(apiClient, "credential") 
        { }
    }
}
