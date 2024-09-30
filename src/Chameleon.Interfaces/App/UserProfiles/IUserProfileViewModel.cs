using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.lib.Common.Enums;
using Chameleon.lib.WebBrowser.Interfaces;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileViewModelBase
{
    IUserProfile UserProfile { get; }
		ISysBrowserInstance SBI { get; }
    void Open();
    void OpenUserBrowser();
    Task OpenSystemBrowser(SystemBrowserType browserType);
}
public interface IUserProfileActionsViewModel : IUserProfileViewModelBase, IPageViewModel
{
}

public interface IUserProfileViewModel : IUserProfileViewModelBase, IPageViewModel
{
    UserProfileViewTab Tab { get; set; }
}
