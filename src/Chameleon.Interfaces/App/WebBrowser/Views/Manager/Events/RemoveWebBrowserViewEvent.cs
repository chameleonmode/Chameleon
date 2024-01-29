using Chameleon.Interfaces.UserProfiles;
using Prism.Events;

namespace Chameleon.Interfaces.WebBrowser
{
    public class RemoveWebBrowserViewEvent
        : PubSubEvent<UserProfileEventArgs>
    { }
}