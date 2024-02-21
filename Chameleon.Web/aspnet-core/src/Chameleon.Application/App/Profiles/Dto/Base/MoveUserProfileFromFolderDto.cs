using System.Collections.Generic;

namespace Chameleon.App
{
    public class MoveUserProfileFromFolderDto
    {
        public List<int> ProfileIds { get; set; }
        public int? FolderId { get; set; }
    }
}
