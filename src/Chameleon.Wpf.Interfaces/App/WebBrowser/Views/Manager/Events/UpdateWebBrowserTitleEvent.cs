

namespace Chameleon.Interfaces.App.WebBrowser.Views.Manager.Events
{
    public class UpdateWebBrowserTitleEvent : PubSubEvent<UpdateWebBrowserTitleEventArgs>
    {
    }

    public class UpdateWebBrowserTitleEventArgs
    {
        public int UserProfileId { get; }

        public UpdateWebBrowserTitleEventArgs(int userProfileId)
        {
            UserProfileId = userProfileId;
        }
    }
}
