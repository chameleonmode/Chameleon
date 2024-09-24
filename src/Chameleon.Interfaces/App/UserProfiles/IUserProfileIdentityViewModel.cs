using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileIdentityViewModel : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
{
    //IUserProfile UserProfile { set; }
}

public interface IUserProfileSidePanelViewModel
{
    IUserProfile UserProfile { set; }
}