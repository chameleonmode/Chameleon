using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachLinkView
        : IViewControl
        , ISingletonDependency
        , IUserProfileAccessor
    {
    }
}
