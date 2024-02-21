using Abp.Domain.Entities;
using Chameleon.Authorization.Users;
using System.Collections.Generic;

namespace Chameleon.App.Entities.ShareFolders
{
    public class UserFolder 
        : Entity,
         IMustHaveUser
    {
        public long UserId { get; set; }
        public virtual User User { get; set; }

        public int FolderId { get; set; }
        public virtual Folder Folder { get; set; }

        public ICollection<UserFolderPermission> UserFolderPermissions { get; set; }
    }
}
