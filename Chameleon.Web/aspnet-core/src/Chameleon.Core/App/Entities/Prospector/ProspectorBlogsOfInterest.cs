using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class ProspectorBlogsOfInterest
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ProspectorBlogsOfInterestTypes Type { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
