using Chameleon.Infrastructure.Proxies.Api.Dto;
using Chameleon.Infrastructure.UserProfiles.Api.Dto.WebBrowser;

namespace Chameleon.Infrastructure.Profiles
{
    public class CreateUserProfileDto
    {
        public string Title { get; set; }
        public bool Favourite { get; set; }
        public int? FolderId { get; set; }
        public WebBrowserDto WebBrowser { get; set; }
        public ProxyDto Proxy { get; set; }
    }
}
