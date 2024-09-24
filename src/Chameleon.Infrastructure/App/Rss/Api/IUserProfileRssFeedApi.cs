using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Rss
{
    public interface IUserProfileRssFeedApi
        : IApiLayer<UserProfileRssFeedDto, int, CreateUserProfileRssFeedDto, UserProfileRssFeedDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
