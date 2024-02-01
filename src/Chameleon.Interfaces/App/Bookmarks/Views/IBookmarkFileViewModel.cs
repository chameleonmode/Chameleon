using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IBookmarkFileViewModel
        : IUserProfileSetter
    {
        int BookmarkFileId { set; get; }
        int ProfileBookmarkId { set; get; }
    }
}
