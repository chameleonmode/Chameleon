using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles
{
    public interface ISharingOptionsForUserProfilePopupViewModel : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
    }
}
