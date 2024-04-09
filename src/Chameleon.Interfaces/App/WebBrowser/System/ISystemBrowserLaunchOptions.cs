using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowserLaunchOptions
    {
        string Url { get; }
        bool SignIn { get; }
        IUserProfile UserProfile { get; }
    }
}
