using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles.Services
{
    public interface IShareUserProfilePopupService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void ShowPopup(IUserProfile UserProfile);
    }
}
