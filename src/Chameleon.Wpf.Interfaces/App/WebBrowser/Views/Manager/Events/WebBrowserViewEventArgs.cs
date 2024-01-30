using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserViewEventArgs : UserProfileEventArgs
    {
        public IWebBrowserView View { get; }

        public WebBrowserViewEventArgs(
            IUserProfile userProfile, 
            IWebBrowserView view
            ) : base(userProfile)
        {
            View = view;
        }
    }
}
