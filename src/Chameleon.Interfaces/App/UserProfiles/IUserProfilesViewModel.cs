using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

//TODO: ???
public interface IUserProfilesViewModel :ISingletonDependency
{
    Func<IUserProfile, bool> Filter { get; set; }
    void Refresh();
    void OnNavigatingTo(IUserProfile p = null);
}
