using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfiles;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Interfaces.WebBrowser
{
    public class SystemBrowserLaunchOptions : ISystemBrowserLaunchOptions
    {
        public string Url { get; set; }
        public bool SignIn { get; set; }
        public IUserProfile UserProfile { get; set; }
        public IUserProfileActionsViewModel UserProfileVM { get; set; }

        public SystemBrowserType BrowserType { get; set; }
    }
}
