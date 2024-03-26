using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Interfaces.App.UserProfiles;
public interface IUserProfileFoldersViewModel
{
    Func<IUserProfileFolder, bool> Filter { get; set; }
    void Refresh();
}
