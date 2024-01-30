namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserCookie
    {
        string Domain { get; set; }
        decimal ExpirationDate { get; set; }
        bool HostOnly { get; set; }
        bool HttpOnly { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        string Path { get; set; }
        string SameSite { get; set; }
        bool Secure { get; set; }
        bool Session { get; set; }
        string StoreId { get; set; }
        string Value { get; set; }
        string Url { get; }
    }
}