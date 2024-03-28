using Chameleon.Interfaces.App.ShareFolders;
using System.Collections.Generic;

namespace Chameleon.Domain.Entities.ShareFolders
{
    public class ShareFolder
        : IShareFolder
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
   
        public List<IShareFolderPermission> FolderPermissions { get; set; } = new List<IShareFolderPermission>();
    }
}
