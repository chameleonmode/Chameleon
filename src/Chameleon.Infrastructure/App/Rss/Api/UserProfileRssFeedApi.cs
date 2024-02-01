using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.Rss
{
    public class UserProfileRssFeedApi
        : ApiLayer<UserProfileRssFeedDto, int, CreateUserProfileRssFeedDto, UserProfileRssFeedDto>
        , IUserProfileRssFeedApi
    {
        public UserProfileRssFeedApi(IApiClient apiClient)
            : base(apiClient, "RSSFeed")
        { }
    }
}
