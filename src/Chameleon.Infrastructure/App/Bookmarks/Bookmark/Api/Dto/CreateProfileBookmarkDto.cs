using Chameleon.Interfaces.Bookmarks;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class CreateProfileBookmarkDto
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public BookmarkType BookmarkType { get; set; }
        public int ProfileId { get; set; }
    }
}
