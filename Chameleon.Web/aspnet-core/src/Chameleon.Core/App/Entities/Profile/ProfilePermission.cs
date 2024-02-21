using Abp.Domain.Entities;
using Chameleon.App.Entities.Assistant;
using Chameleon.App.Entities.ShareFolders;
using System.Collections.Generic;

namespace Chameleon.App.Entities.Permissions
{
    public class ProfilePermission 
        : Entity
    {
        public string PermissionName { get; set; }
        public string DisplayName { get; set; }

        public virtual ICollection<ProfileAssistantPermission> ProfileAssistantPermissions { get; set; }

        public virtual ICollection<UserFolderPermission> UserFolderPermissions { get; set; }
    }
}
