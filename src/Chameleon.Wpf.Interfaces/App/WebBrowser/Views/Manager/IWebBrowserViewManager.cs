using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserViewManager : ISingletonDependency
    {
        IWebBrowserView GetOrCreateView(IUserProfile userProfile);
    }
}
