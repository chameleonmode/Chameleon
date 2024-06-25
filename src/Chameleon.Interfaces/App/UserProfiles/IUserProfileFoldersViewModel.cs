using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;
public interface IUserProfileFoldersViewModel
{
    void SetSelectedById(int id);
    Func<IUserProfileFolder, bool> Filter { get; set; }
    void Refresh();
    Task OnNavigatingTo(IUserProfileFolder p = null);
}
