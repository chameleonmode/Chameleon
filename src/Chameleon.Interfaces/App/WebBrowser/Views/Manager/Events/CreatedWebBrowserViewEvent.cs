using Prism.Events;

namespace Chameleon.Interfaces.WebBrowser
{
    public class CreatedWebBrowserViewEvent
        : PubSubEvent<WebBrowserViewEventArgs>
    { }
}