using Chameleon.Interfaces.Entities;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.ShareFolders
{
    public interface IShareFolder
        : IEntity
    {
        long UserId { get; set; }
        int FolderId { get; set; }
        string FolderName { get; set; }
        IList<IShareFolderPermission> FolderPermissions { get; set; }
    }
}
