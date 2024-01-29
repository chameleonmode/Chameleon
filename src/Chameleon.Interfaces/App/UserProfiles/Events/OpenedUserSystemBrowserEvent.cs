using Chameleon.Interfaces.WebBrowser;
using Prism.Events;
using System;
using System.Diagnostics;

namespace Chameleon.Interfaces.UserProfiles
{
    public class UserProfileSystemBrowserProcessEventArgs
        : UserProfileSystemBrowserEventArgs
    {
        public Process Process { get; }

        public UserProfileSystemBrowserProcessEventArgs(
            IUserProfile userProfile,
            SystemBrowserType browserType,
            Process process,
            Uri url = null,
            bool signin = false
            ) : base(userProfile, browserType, url, signin)
        {
            Process = process;
        }
    }

    public class OpenedUserSystemBrowserEvent
        : PubSubEvent<UserProfileSystemBrowserProcessEventArgs>
    {
    }
}