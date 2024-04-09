using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class SystemBrowserLaunchOptions : ISystemBrowserLaunchOptions
    {
        public string Url { get; set; }
        public bool SignIn { get; set; }
        public IUserProfile UserProfile { get; set; }
    }
}
