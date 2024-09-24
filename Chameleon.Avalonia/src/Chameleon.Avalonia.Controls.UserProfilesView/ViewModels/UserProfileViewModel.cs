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
using Chameleon.lib.Common.Enums;
using Chameleon.lib.Common.Models;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileViewModel : SubPageViewModelBase, IUserProfileActionsViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IApplicationUser _applicationUser;

    [ObservableProperty]
    private UserProfile _userProfile;
    [ObservableProperty]
    private string _isChromeRunning = "False";
    [ObservableProperty]
    private string _isBraveRunning = "False";
    [ObservableProperty]
    private string _isFFRunning = "False";
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
        bool isShowCheckboxColumn = true,
        bool isShowGlyph = true,
        bool isShowC = true,
        bool isShowD = true,
        bool isShowF = true
        )
    {
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
           .Subscribe(a =>
           {
               if (a.UserProfile.Id != UserProfile.Id)
                   return;

               SetRunning(a, true);
           });

        EventAggregator.Sub<OpenedUserSystemBrowserErrorEvent, UserProfileSystemBrowserProcessEventArgs>(a =>
        {
            if (a.UserProfile.Id != UserProfile.Id)
                return;

            _systemBrowserInstance = null;
            IsForeground = false;
            SetRunning(a, null);
        });

        EventAggregator
            .GetEvent<ClosedUserSystemBrowserEvent>()
            .Subscribe(a =>
            {
                if (a.UserProfile.Id != UserProfile.Id)
                    return;

                _systemBrowserInstance = null;
                var runnin = SetRunning(a, false);
                IsForeground = runnin != "Error" && runnin != "False";
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
    string SetRunning(UserProfileSystemBrowserProcessEventArgs args, bool? running) => (SystemBrowserType.Unknown) switch
    {
        SystemBrowserType.Chrome => running != true && IsChromeRunning == "Error" ? "Error" 
        : IsChromeRunning = UserProfile.IsChromeRunning = running is null ? "Error" : running == true ? "True" : "False",
       
        SystemBrowserType.Firefox => running != true && IsFFRunning == "Error" ? "Error" 
        : IsFFRunning = UserProfile.IsFFRunning= running is null ? "Error" : running == true ? "True" : "False",
        
        SystemBrowserType.Brave => running != true && IsBraveRunning == "Error" ? "Error"
        : IsBraveRunning = UserProfile.IsBraveRunning = running is null ? "Error" : running == true ? "True" : "False",

        _ => "Error"
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
    public async Task OpenSystemBrowser(object browserType)
    {
        //_systemBrowserInstance 
        //         .Open(new SystemBrowserLaunchOptions
        //         {
        //             BrowserType = browserType,
        //         });
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