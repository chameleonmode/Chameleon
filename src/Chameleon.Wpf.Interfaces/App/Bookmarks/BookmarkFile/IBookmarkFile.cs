using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IBookmarkFile
        : IEntity<int>
    {
        string Url { set; get; }
        string Name { set; get; }
        int BookmarkId { get; set; }
    }
}
