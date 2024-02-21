namespace Chameleon.App
{
    public class ProfileBaseDto
    {
        public string Title { get; set; }
        public string Notes { get; set; }
        public bool IsFavourite { get; set; }
        public int? FolderId { get; set; }
        public double? LimitCache { get; set; }
        public string YoutubeApiKey { get; set; }
        public string YoutubeClientId { get; set; }
        public string YoutubeClientSecret { get; set; }
        public WordPressSettingsDto WordPressSettings { get; set; }
        public int ProxyId { get; set; }
        public ProxyBaseDto Proxy { get; set; }
        public WebBrowserSettingBaseDto WebBrowser { get; set; }
    }
}
