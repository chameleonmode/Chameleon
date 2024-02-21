using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class OutReachTemplate 
        : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
    }
}
