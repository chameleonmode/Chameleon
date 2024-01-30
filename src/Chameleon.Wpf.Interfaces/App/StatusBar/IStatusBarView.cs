using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.StatusBar
{
    public interface IStatusBarView : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
