using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.App.Rss.Events
{
    public class WebBrowserDisposeEventArgs : EventArgs
    {
        public IList<string> Links;

        public WebBrowserDisposeEventArgs(
            IList<string> links)
        {
            Links = links;
        }
    }
}