using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IProfileBookmarkService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IProfileBookmarks GetAll(int profileId, bool ignoreCache = false);
        void Update(IProfileBookmark bookmark);
        void Delete(IProfileBookmark bookmark);
        IProfileBookmark Create(IProfileBookmark bookmark);
    }
}
