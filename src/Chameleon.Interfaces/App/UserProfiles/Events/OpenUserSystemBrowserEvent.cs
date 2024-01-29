using Chameleon.Interfaces.WebBrowser;
using Prism.Events;
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
            Uri url = null,
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