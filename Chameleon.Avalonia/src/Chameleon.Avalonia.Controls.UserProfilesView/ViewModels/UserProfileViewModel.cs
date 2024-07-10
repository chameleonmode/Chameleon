using Chameleon.Authorization;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileViewModel : SubPageViewModelBase, IUserProfileActionsViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IApplicationUser _applicationUser;
    private readonly ISystemBrowserManager _systemBrowserManager;

    [ObservableProperty]
    private UserProfile _userProfile;
    [ObservableProperty]
    private bool _isChromeRunning;
    [ObservableProperty]
    private bool _isBraveRunning;
    [ObservableProperty]
    private bool _isFFRunning;
    [ObservableProperty]
    private bool _isShowGlyph;
    [ObservableProperty]
    private bool _isShowC;
    [ObservableProperty]
    private bool _isShowD;
    [ObservableProperty]
    private bool _isShowF;
    [ObservableProperty]
    private bool _isForeground;

    private ISystemBrowserInstance _systemBrowserInstance;

    public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];
    public bool IsFavorite => UserProfile?.IsFavourite ?? false;
    public bool IsSharedProfile => _userProfileService.IsSharedProfile(UserProfile);
    public bool IsShowCheckboxColumn { get; }
    public bool IsEnabledCheckboxColumn { get; }
    public bool IsDeleteProfileBtnVisible => !IsSharedProfile && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
    public bool IsOutreachBtnEnabled => !IsSharedProfile || UserProfile.HasPermission(PermissionNames.Pages_Outreach);
    public bool IsRssBtnEnabled => !IsSharedProfile || UserProfile.HasPermission(PermissionNames.Pages_RSS);

    public ISystemBrowserInstance SBI => _systemBrowserInstance;

    IUserProfile IUserProfileViewModelBase.UserProfile => UserProfile;

    public UserProfileViewModel(
        IUserProfileService userProfileService,
        UserProfile userProfile,
        IApplicationUser applicationUser,
        ISystemBrowserManager systemBrowserManager,
        bool isShowCheckboxColumn = true,
        bool isShowGlyph = true,
        bool isShowC = true,
        bool isShowD = true,
        bool isShowF = true
        )
    {
        _systemBrowserManager = systemBrowserManager;
        _userProfileService = userProfileService;
        _applicationUser = applicationUser;
        _userProfile = userProfile;

        Title = _userProfile.Title;

        IsChromeRunning = _userProfile.IsChromeRunning;
        IsBraveRunning = _userProfile.IsBraveRunning;
        IsFFRunning = _userProfile.IsFFRunning;
        IsShowGlyph = isShowGlyph;
        IsShowD = isShowD;
        IsShowC = isShowC;
        IsShowF = isShowF;
        IsShowCheckboxColumn = isShowCheckboxColumn && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
        IsEnabledCheckboxColumn = !_userProfileService.IsSharedProfile(_userProfile);

        EventAggregator
           .GetEvent<OpenedUserSystemBrowserEvent>()
           .Subscribe(a => SetRunning(a, true));

        EventAggregator
            .GetEvent<ClosedUserSystemBrowserEvent>()
            .Subscribe(a =>
            {
                _systemBrowserInstance = null;
                IsForeground = SetRunning(a, false);
            });

        EventAggregator.GetEvent<ForegroundUserSystemBrowserEvent>().Subscribe(a =>
            {
                if (a.UserProfile.Id == UserProfile.Id)
                    IsForeground = true;
                else
                    IsForeground = false;
            });

        EventAggregator.GetEvent<SavedUserProfileEvent>().Subscribe(a =>
        {
            if (a.UserProfile.Id == UserProfile.Id)
            {
                Title = _userProfile.Title;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Code));
            }
        });
    }

    void SetForgroung(UserProfileSystemBrowserProcessEventArgs args)
    {

    }
    bool SetRunning(UserProfileSystemBrowserProcessEventArgs args, bool running) => args.UserProfile.Id == UserProfile.Id && args.BrowserType switch
    {
        SystemBrowserType.Chrome => IsChromeRunning = UserProfile.IsChromeRunning = running,
        SystemBrowserType.Firefox => IsFFRunning = UserProfile.IsFFRunning = running,
        SystemBrowserType.Brave => IsBraveRunning = UserProfile.IsBraveRunning = running,
        _ => false
    };

    [RelayCommand]
    private void ShowViewProfile()
    {
        ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<IUserProfileSidePanelView, IUserProfileSidePanelViewModel>(vm =>
        {
            vm.UserProfile = UserProfile;
        }, null, "Copy Pasta", 156);
    }

    [RelayCommand]
    private void Favorite()
    {
        if (!IsFavorite)
        {
            UserProfile.IsFavourite = true;
            EventAggregator
                .GetEvent<FavoriteUserProfileEvent>()
                .Publish(new UserProfileEventArgs(UserProfile));

        }
        else
        {
            UserProfile.IsFavourite = false;
            EventAggregator
                .GetEvent<UnfavoriteUserProfileEvent>()
                .Publish(new UserProfileEventArgs(UserProfile));

        }

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();

        OnPropertyChanged(nameof(IsFavorite));
    }
    [RelayCommand]
    private async Task DeleteUserProfile()
    {
        if (await MesageBoxHelper.ShowAsync("Delete User Profile",
          $"Are you sure you want to delete {UserProfile.Title}?",
          ContentDialogButtons.YesNo,
          "DeleteLines"))
            EventAggregator
             .GetEvent<DeleteUserProfileEvent>()
             .Publish(new UserProfileEventArgs(UserProfile));
    }
    public void Open()
    {
        NavigationService.NavigateToType(typeof(IUserProfileIdentityView), UserProfile);
        //OpenUserProfile();
    }

    [RelayCommand]
    private void OpenUserProfile()
    {
        Open();
    }
    [RelayCommand]
    public void OpenUserBrowser()
    {
        ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<ITopMostSidePanelView, ITopMostSidePanelViewModel>(
            vm =>
            {
                if (!vm.RunningList.Contains(this))
                    vm.RunningList.Add(this);

                vm.Update();
            },
            vm =>
            {
                vm.RunningList.Clear();
            }, "SCP", 172);
    }
    [RelayCommand]
    private async Task OpenFirefox()
    {
        await OpenSystemBrowser(SystemBrowserType.Firefox);
    }
    [RelayCommand]
    private async Task OpenChrome()
    {
        await OpenSystemBrowser(SystemBrowserType.Chrome);
    }
    [RelayCommand]
    private async Task OpenBrave()
    {
        await OpenSystemBrowser(SystemBrowserType.Brave);
    }
    [RelayCommand]
    public async Task OpenSystemBrowser(SystemBrowserType browserType)
    {
        _systemBrowserInstance =
             await _systemBrowserManager.Get(browserType)
                 .Open(new SystemBrowserLaunchOptions
                 {
                     Url = null,
                     SignIn = false,
                     UserProfile = UserProfile,
                     BrowserType = browserType,
                 });
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                EventAggregator
                    .GetEvent<SelectedChangeUserProfileEvent>()
                    .Publish(new SelectedUserProfileEventArgs(UserProfile, _isSelected));
            }
        }
    }
}