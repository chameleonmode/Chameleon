using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Rss
{
    public class UserProfileRssFeedDto 
        : CreateUserProfileRssFeedDto
        , IEntityDto
    {
        public int Id { get; set; }
    }
}
