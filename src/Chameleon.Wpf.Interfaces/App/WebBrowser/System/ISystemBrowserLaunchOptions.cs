using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowserLaunchOptions
    {
        Uri Url { get; }
        bool SignIn { get; }
        IUserProfile UserProfile { get; }
    }
}
