using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IAddUserProfilesPopupViewModel
        : ITransientDependency
    {
        IUserProfileFolder Folder { get; set; }
    }
}
