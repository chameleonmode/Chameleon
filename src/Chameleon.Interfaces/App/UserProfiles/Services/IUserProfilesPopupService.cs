using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfilesPopupService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void ShowPopup(IUserProfileFolder folder);
    }
}
