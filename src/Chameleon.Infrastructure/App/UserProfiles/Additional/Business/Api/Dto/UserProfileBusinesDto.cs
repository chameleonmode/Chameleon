using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{

    public class UserProfileBusinessDto
        : CreateUserProfileBusinessDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
