using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileViewModel
{
    IUserProfile UserProfile { get; set; }
    UserProfileViewTab Tab { get; set; }
}
