using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.OutReach.Api
{
    public class UserProfileApiOutReachRss
        : ApiLayer<
            UserProfileOutReachRssDto
            , int
            , CreateUserProfileOutReachRssDto
            , UserProfileOutReachRssDto
            >
        , IUserProfileApiOutReachRss
    {
        public UserProfileApiOutReachRss(IApiClient apiClient)
            : base(apiClient, "outreachrss")
        { }
    }
}
