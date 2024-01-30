using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles
{
    public interface ISharingOptionsForUserProfilePopupViewModel : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
