using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserEventArgs : EventArgs
    {
        public Uri Url { get; }
        public IUserProfile UserProfile { get; }

        public WebBrowserEventArgs(
            IUserProfile userProfile, 
            Uri url)
        {
            Url = url;
            UserProfile = userProfile;
        }
    }
}
