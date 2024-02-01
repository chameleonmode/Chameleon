using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileDto 
        : UserProfileBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
