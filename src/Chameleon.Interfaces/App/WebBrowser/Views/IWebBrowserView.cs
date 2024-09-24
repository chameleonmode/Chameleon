using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserView : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
        //IWebBrowserInstance Browser { get; }
        void OpenWebBrowserNewTab(string url);
        void SetBookmark();
    }
}
