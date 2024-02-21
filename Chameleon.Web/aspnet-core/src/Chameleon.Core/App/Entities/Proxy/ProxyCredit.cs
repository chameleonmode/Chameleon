using Abp.Domain.Entities.Auditing;
using Chameleon.Authorization.Users;

namespace Chameleon.App.Entities
{
    public class ProxyCredit 
        : FullAuditedEntity
        , IMustHaveUser
    {
        public string ProxyUserName { get; set; }
        public string ProxyAuthKey { get; set; }
        public ProxyProviderType ProviderType { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
