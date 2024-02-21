using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class CookiesExcludedDomain 
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Domain { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
