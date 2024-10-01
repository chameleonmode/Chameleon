using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfiles;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowserLaunchOptions
    {
        string Url { get; }
        bool SignIn { get; }

        public IUserProfile UserProfile { get; }
        public IUserProfileActionsViewModel UserProfileVM { get; }
        SystemBrowserType BrowserType { get; }
    }
}
