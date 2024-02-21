using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class Credential 
        : FullAuditedEntity
        , IMustHaveProfile
    {
        public string Title { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
