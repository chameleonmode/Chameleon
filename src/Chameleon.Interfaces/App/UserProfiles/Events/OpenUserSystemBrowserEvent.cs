using Chameleon.Interfaces.WebBrowser;

using System;

namespace Chameleon.Interfaces.UserProfiles
{
    public class UserProfileSystemBrowserEventArgs : UserProfileEventArgs
    {
        public bool SignIn { get; }
        public SystemBrowserType BrowserType { get; }

        public UserProfileSystemBrowserEventArgs(
            IUserProfile userProfile,
            SystemBrowserType browserType,
            string? url = null,
            bool signin = false
            ) : base(userProfile, url)
        {
            SignIn = signin;
            BrowserType = browserType;
        }
    }

    public class OpenUserSystemBrowserEvent
        : PubSubEvent<UserProfileSystemBrowserEventArgs>
    { }
}