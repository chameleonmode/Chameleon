using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Chameleon.App.ValueObjects;
using Chameleon.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chameleon.App.Entities
{
    public class AssistantLicense : FullAuditedEntity
    {
        public const string DefaultTenantName = AbpTenantBase.DefaultTenantName;

        [Column(nameof(LicenseKey))]
        public string LicenseKeyValue
        {
            get => LicenseKey;
            set => LicenseKey = AssistantLicenseKey.Create(value);
        }

        [NotMapped]
        public AssistantLicenseKey LicenseKey { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }

        public long PrimaryUserId { get; set; }
        public virtual User PrimaryUser { get; set; }

        public bool CanCreateProfiles { get; set; }
    }
}
