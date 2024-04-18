using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileViewModelBase
{
    IUserProfile UserProfile { get; set; }
    void Open();
}

public interface IUserProfileViewModel : IUserProfileViewModelBase
{
    UserProfileViewTab Tab { get; set; }
}
