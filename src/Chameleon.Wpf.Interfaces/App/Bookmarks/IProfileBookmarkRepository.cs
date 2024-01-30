using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IProfileBookmarkRepository
        : IRepository<IProfileBookmark, int, UserProfileGetAllRequestDto>
    {
    }
}
