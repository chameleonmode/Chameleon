using System.Collections.Generic;

namespace Chameleon.App.ShareFolders.Dto
{
    public class CreateShareFolderDto
    {
        [Identity]
        public long UserId { get; set; }
        public IList<int> FolderIds { get; set; }
        public IList<int> PermissionIds { get; set; }
    }
}
