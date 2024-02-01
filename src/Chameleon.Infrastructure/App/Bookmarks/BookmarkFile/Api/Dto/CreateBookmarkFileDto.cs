using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class CreateBookmarkFileDto
    {
        public string Url { set; get; }
        public string Name { set; get; }
        public int BookmarkId { get; set; }
    }
}
