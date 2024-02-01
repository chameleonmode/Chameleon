using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Profiles
{
    public class MoveUserProfileFromFolderDto
    {
        public List<int> ProfileIds { get; set; }
        public int? FolderId { get; set; }
    }
}
