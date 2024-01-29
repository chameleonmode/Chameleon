using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfilesPopupService
        : ISingletonDependency
    {
        void ShowPopup(IUserProfileFolder folder);
    }
}
