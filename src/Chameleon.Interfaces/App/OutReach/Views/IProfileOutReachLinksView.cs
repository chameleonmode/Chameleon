using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.OutReach
{
    public interface IProfileOutReachLinksView
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
        , IUserProfileAccessor
    {
    }
}
