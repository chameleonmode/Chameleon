using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class OutReachRssBaseDto
    {
        public string RssName { get; set; }
        public string RssLink { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public OutReachRssStatus Status { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
