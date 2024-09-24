using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Bookmarks
{
    public interface IBookmarkFileApi
        : IApiLayer<
            BookmarkFileDto
            , int
            , CreateBookmarkFileDto
            , BookmarkFileDto
            >
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
