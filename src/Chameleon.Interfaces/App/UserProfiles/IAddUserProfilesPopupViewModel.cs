using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IAddUserProfilesPopupViewModel
        : ITransientDependency , IContentDialogViewModel
    {
        IUserProfileFolder Folder { get; set; }
    }
}
