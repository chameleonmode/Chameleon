namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserSettings
    {
        bool WebRTC { get; set; }
        bool WebGL { get; set; }
        bool Tracking { get; set; }
        bool Flash { get; set; }
        decimal Canvas { get; set; }
        int? UserAgentId { get; set; }
        IWebBrowserUserAgent UserAgent { get; set; }
    }
}
