using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;
public interface IUserProfileFoldersViewModel
{
    Func<IUserProfileFolder, bool> Filter { get; set; }
    void Refresh();
    void OnNavigatingTo(IUserProfileFolder p = null);
}
