using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.WebBrowser.Interfaces;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Interfaces.App.UserProfiles;

public interface IUserProfileViewModelBase
{
    IUserProfile UserProfile { get; }
	 Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI { get; }
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
