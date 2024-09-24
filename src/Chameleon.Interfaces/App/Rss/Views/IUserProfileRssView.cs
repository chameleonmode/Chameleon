using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Rss
{
    public interface IUserProfileRssView
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
        , IUserProfileAccessor
    {
    }
}
