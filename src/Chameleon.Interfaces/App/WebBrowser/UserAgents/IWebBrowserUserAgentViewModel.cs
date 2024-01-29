namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserUserAgentViewModel 
        : IWebBrowserUserAgent
    {
        bool IsSelected { get; set; }
    }
}
