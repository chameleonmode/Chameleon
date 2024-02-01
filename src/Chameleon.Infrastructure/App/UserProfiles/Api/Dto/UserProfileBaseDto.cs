using Chameleon.Infrastructure.Proxies.Api.Dto;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.WebBrowser;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.WordPressSettings;

namespace Chameleon.Infrastructure.Profiles
{
    public class UserProfileBaseDto
    {
        public string Title { get; set; }
        public string Notes { get; set; }
        public string YoutubeApiKey { get; set; }
        public string YoutubeClientId { get; set; }
        public string YoutubeClientSecret { get; set; }
        public WordPressSettingsDto WordPressSettings { get; set; }

        public bool IsFavourite { get; set; }
        public int? FolderId { get; set; }
        public long? CreatorUserId { get; set; }
        public double? LimitCache { get; set; }
        public ProxyDto Proxy { get; set; }
        public WebBrowserDto WebBrowser { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
