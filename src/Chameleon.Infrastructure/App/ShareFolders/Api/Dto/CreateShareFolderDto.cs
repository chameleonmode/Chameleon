using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.ShareFolders.Api.Dto
{
    public class CreateShareFolderDto
    {
        public long UserId { get; set; }
        public IList<int> FolderIds { get; set; }
        public IList<int> PermissionIds { get; set; }
    }
}
