using Abp.Domain.Entities;
using Chameleon.App.Entities.Permissions;

namespace Chameleon.App.Entities.ShareFolders
{
    public class UserFolderPermission 
        : Entity
    {
        public int UserFolderId { get; set; }
        public virtual UserFolder UserFolder { get; set; }

        public int ProfilePermissionId { get; set; }
        public virtual ProfilePermission ProfilePermission { get; set; }
    }
}
