using Chameleon.Interfaces.OutReach;

namespace Chameleon.Domain.Entities
{
    public class UserProfileOutReachRss
        : IUserProfileOutReachRss
    {
        public string RssName { get; set; }
        public string RssLink { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public OutReachRssStatus Status { get; set; }
        public int ProfileId { get; set; }
        public int Id { get; set; }
    }
}
