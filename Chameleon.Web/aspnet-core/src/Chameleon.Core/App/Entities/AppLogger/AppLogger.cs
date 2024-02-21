using Abp.Domain.Entities.Auditing;
using Chameleon.Authorization.Users;

namespace Chameleon.App.Entities
{
    public class AppLogger 
        : FullAuditedEntity
    {
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public virtual User User { get; set; }

        public string Message { get; set; }
        public AppLoggerType AppLoggerType { get; set; }
    }
}
