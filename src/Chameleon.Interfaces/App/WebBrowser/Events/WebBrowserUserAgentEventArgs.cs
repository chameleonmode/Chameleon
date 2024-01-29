using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserUserAgentEventArgs : EventArgs
    {
        public IWebBrowserUserAgent WebBrowserUserAgent { get; }
        public WebBrowserUserAgentEventArgs(IWebBrowserUserAgent userAgent)
        {
            WebBrowserUserAgent = userAgent;
        }
    }
}
