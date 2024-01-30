using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.Prospector
{
    public interface IBlogOfInterestView
       : IViewControl
       , ISingletonDependency
       , IUserProfileAccessor
    {
    }
}
