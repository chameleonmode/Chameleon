using Chameleon.App.Entities;

namespace Chameleon.App.ShareFolders.Dto.Base
{
    public class ShareFolderBaseDto : IMustHaveUser
    {
        [Identity]
        public long UserId { get; set; }

        [Identity]
        public int FolderId { get; set; }
    }
}
