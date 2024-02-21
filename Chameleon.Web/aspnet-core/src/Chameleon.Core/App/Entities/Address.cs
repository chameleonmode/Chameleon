using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class Address 
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Notes { get; set; }
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
