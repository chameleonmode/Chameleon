using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Interfaces.WebBrowser {
	public class RemoveWebBrowserViewEvent
        : PubSubEvent<UserProfileEventArgs>
    { }
}