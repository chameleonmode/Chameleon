using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class RSSFeed 
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Url { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
