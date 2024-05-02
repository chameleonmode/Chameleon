using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileIdentityViewModel 
{
    //IUserProfile UserProfile { set; }
}

public interface IUserProfileSidePanelViewModel
{
    IUserProfile UserProfile { set; }
}