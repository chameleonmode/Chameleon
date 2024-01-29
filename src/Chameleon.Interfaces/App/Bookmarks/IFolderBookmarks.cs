using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IFolderBookmarks
        : IUserProfileAccessor
    {
        int ProfileBookmarkId { get; set; }
        BookmarkType BookmarkType { get; set; }
    }
}
