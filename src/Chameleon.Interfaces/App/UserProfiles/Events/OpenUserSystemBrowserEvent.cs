using Chameleon.Interfaces.WebBrowser;
using Chameleon.lib.Common.Interfaces.Sys;

using System;

namespace Chameleon.Interfaces.UserProfiles {
	public class UserProfileSystemBrowserEventArgs : UserProfileEventArgs
    {
        public bool SignIn { get; }

        public UserProfileSystemBrowserEventArgs(
            IUserProfile userProfile,
            string? url = null,
            bool signin = false
            ) : base(userProfile, url)
        {
            SignIn = signin;
        }
    }

    public class OpenUserSystemBrowserEvent
        : PubSubEvent<UserProfileSystemBrowserEventArgs>
    { }
}