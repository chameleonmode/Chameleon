using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.OutReach.Api
{
    public interface IUserProfileApiOutReachRss
        : IApiLayer<
            UserProfileOutReachRssDto
            , int
            , CreateUserProfileOutReachRssDto
            , UserProfileOutReachRssDto
            >
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
