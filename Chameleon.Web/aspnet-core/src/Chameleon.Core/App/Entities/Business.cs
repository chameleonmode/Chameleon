using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class Business 
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public string Notes { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
