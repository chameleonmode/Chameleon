using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.UserProfiles.Api.Additional
{
    public class UserProfileApiAddress 
        : ApiLayer<UserProfileAddressDto, int, CreateUserProfileAddressDto, UserProfileAddressDto>
        , IUserProfileApiAddress
    {
        public UserProfileApiAddress(IApiClient apiClient) 
            : base(apiClient, "address")
        { }
    }
}
