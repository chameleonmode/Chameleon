using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class UserProfileOutReachRssDto
        : CreateUserProfileOutReachRssDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
