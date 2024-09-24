using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Bookmarks
{
    public interface IProfileBookmarkApi
        : IApiLayer<
            ProfileBookmarkDto
            , int
            , CreateProfileBookmarkDto
            , ProfileBookmarkDto
            >
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
