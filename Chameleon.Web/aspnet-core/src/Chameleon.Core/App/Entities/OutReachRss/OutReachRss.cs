using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class OutReachRss
        : FullAuditedEntity
        , IMustHaveProfile
    {        
        public string RssName { get; set; }
        public string RssLink { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public OutReachRssStatus Status { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
