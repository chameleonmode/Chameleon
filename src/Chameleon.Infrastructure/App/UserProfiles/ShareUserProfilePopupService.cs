using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.App.UserProfiles;

public class ShareUserProfilePopupService : IShareUserProfilePopupService
{
    private readonly IDialogWindowsService _dialogWindowsService;
    private readonly IHaveContainerProvider _containerProvider;

    public ShareUserProfilePopupService(
        IDialogWindowsService dialogWindowsService
        , IHaveContainerProvider containerProvider
        )
    {
        _dialogWindowsService = dialogWindowsService;
        _containerProvider = containerProvider;
    }

    public void ShowPopup(IUserProfile userProfile)
    {
        var title = "SHARING OPTIONS FOR " + userProfile.Title.ToUpper();
        var popup = _containerProvider.Resolve<ISharingOptionsForUserProfilePopupView>();
        popup.UserProfile = userProfile;

        _dialogWindowsService.ShowDialogWindow(popup, title);
    }
}
