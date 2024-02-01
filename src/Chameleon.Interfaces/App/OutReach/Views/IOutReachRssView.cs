using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachRssView
        : IViewControl
        , ISingletonDependency
        , IUserProfileAccessor
    {
    }
}
