using Abp.Domain.Entities;
using Chameleon.App.Entities.Permissions;

namespace Chameleon.App.Entities.Assistant
{
    public class ProfileAssistantPermission 
        : Entity<long>
    {
        public long ProfileAssistantId { get; set; }
        public virtual ProfileAssistant ProfileAssistant { get; set; }

        public int ProfilePermissionId { get; set; }
        public virtual ProfilePermission ProfilePermission { get; set; }
    }
}
