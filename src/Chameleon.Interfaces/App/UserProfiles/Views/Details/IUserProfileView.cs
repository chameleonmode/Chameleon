using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileView
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
        , IUserProfileGetter
        , IViewContentContext
    {
        UserProfileViewTab Tab { get; }
        IWebBrowserView WebBrowserView { get; }
        //TODO: IWebBrowserInstance WebBrowser { get; }
        void OpenWebBrowserNewTab(string url);

        void SetUserProfile(
            IUserProfile userProfile,
            UserProfileViewTab tab,
            OutReachViewTab outReachTab = OutReachViewTab.Rss);
    }
}
