using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class UserProfileLoginDto
        : CreateUserProfileLoginDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
