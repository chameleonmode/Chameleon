using Abp.Domain.Entities.Auditing;
using Chameleon.Authorization.Users;

namespace Chameleon.App.Entities
{
    public class UserSettings : FullAuditedEntity
    {
        public long? UserId { get; set; }
        public virtual User User { get; set; }

        public string SmsPvaApiKey { get; set; }
    }
}
