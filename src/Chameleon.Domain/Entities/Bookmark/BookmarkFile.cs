using Chameleon.Interfaces.Bookmarks;

namespace Chameleon.Domain.Entities
{
    public class BookmarkFile:
        IBookmarkFile
    {
        public int Id { get; set; }
        public string Url { set; get; }
        public string Name { set; get; }
        public int BookmarkId { set; get; }
    }
}
