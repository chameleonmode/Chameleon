using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles.Services
{
    public interface IShareUserProfilePopupService : ISingletonDependency
    {
        void ShowPopup(IUserProfile UserProfile);
    }
}
