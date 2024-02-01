using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Prospector.Api.Dto
{
    public class UserProfileProspectorBlogsOfInterestDto
        : CreateUserProfileProspectorBlogsOfInterestDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
