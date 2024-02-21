using Abp.Domain.Entities.Auditing;
using Chameleon.Authorization.Users;
using System;

namespace Chameleon.App.Entities
{
    public class ProxyCreditOrder
        : FullAuditedEntity<Guid>
        , IMustHaveUser
    {
        public decimal Amount { get; set; }
        public string ExternalId { get; set; }
        public string ExternalStatus { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string ExternalInvoiceId { get; set; }
        public string ExternalCaptureId { get; set; }

        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
