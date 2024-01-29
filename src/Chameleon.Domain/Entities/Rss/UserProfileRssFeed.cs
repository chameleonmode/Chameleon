using Chameleon.Interfaces.Rss;

namespace Chameleon.Domain.Entities
{
    public class UserProfileRssFeed 
        : IUserProfileRssFeed
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProfileId { get; set; }
    }
}
