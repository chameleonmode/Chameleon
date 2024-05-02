using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileViewModelBase
{
    IUserProfile UserProfile { get; set; }
    void Open();
}
public interface IUserProfileActionsViewModel : IUserProfileViewModelBase, IPageViewModel
{
}

public interface IUserProfileViewModel : IUserProfileViewModelBase, IPageViewModel
{
    UserProfileViewTab Tab { get; set; }
}
