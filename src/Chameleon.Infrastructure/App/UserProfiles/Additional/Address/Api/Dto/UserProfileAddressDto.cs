using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfileAddressDto 
        : CreateUserProfileAddressDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
