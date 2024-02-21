using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Chameleon.App.ValueObjects;
using Chameleon.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chameleon.App.Entities
{
    public class License : FullAuditedEntity
    {
        public const string DefaultTenantName = AbpTenantBase.DefaultTenantName;

        [Column(nameof(LicenseKey))]
        public string LicenseKeyValue
        {
            get => LicenseKey;
            set => LicenseKey = LicenseKey.Create(value);
        }

        [NotMapped]
        public LicenseKey LicenseKey { get; set; }
        public bool IsActive { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
