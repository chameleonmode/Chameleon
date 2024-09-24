using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.Bookmarks
{
    public interface IBookmarkView   
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
        , IUserProfileAccessor
    { 
    }
}
