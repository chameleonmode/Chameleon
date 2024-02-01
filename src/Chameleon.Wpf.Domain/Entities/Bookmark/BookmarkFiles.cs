using Chameleon.Interfaces.Bookmarks;

namespace Chameleon.Domain.Entities
{
    public class BookmarkFiles
        : ObservableEntities<IBookmarkFile>
        , IBookmarkFiles
    {
    }
}
