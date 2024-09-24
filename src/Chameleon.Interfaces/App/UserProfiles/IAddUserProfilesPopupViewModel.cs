using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IAddUserProfilesPopupViewModel
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency , IContentDialogViewModel
    {
        IUserProfileFolder Folder { get; set; }
    }
}
