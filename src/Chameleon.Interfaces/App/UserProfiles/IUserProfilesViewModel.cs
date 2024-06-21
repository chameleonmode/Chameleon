using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

//TODO: ???
public interface IUserProfilesViewModel :ISingletonDependency
{                           
    Func<IUserProfile, bool> Filter { get; set; }
    string SearchText { get; set; }
    Task<IUserProfile> CreateNewProfile();
    void Open(IUserProfileFolder? folder);
    void OnFilterTo(IUserProfile p = null);
}
