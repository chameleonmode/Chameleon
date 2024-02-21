using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class WebBrowserUserAgent 
        : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsDefault { get; set; }
    }
}
