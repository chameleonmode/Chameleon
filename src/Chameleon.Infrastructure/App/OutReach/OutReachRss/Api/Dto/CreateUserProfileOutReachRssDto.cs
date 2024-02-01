namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class CreateUserProfileOutReachRssDto
    {
        public string RssName { get; set; }
        public string RssLink { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public int ProfileId { get; set; }
    }
}
