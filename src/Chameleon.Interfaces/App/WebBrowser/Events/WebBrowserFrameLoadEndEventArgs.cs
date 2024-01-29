using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public class WebBrowserFrameLoadEndEventArgs : EventArgs
    {
        public WebBrowserFrameLoadEndEventArgs(
            string url, 
            int httpStatusCode,
            bool isMain
            )
        {
            Url = url;
            HttpStatusCode = httpStatusCode;
            IsMain = isMain;
        }

        public bool IsMain { get; }
        public string Url { get; }
        public int HttpStatusCode { get; set; }
    }
}
