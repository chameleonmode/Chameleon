using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserViewManager : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IWebBrowserView GetOrCreateView(IUserProfile userProfile);
    }
}
