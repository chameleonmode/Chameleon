using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfiles;
using System;

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
