using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class ProxyCreditPlan
        : FullAuditedEntity
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
    }
}
