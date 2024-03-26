using Chameleon.Authorization;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.ContentDiscoverey;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using System.Drawing;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileViewModel : SubPageViewModelBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfile _userProfile;
    //private readonly IPrismMessageBoxService _messageBoxService;
    //private readonly IViewProfileWindowService _viewProfileWindowService;
    private readonly IShareUserProfilePopupService _shareUserProfilePopupService;
    private readonly IApplicationUser _applicationUser;

    public UserProfileViewModel(
        IUserProfileService userProfileService,
        IUserProfile userProfile,
        IShareUserProfilePopupService shareUserProfilePopupService,
        IApplicationUser applicationUser,
        bool isShowCheckboxColumn = true
        )
    {
        _userProfileService = userProfileService;
        _userProfile = userProfile;
        _shareUserProfilePopupService = shareUserProfilePopupService;
        _applicationUser = applicationUser;   

        Title = _userProfile.Title;

        IsShowCheckboxColumn = isShowCheckboxColumn && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
        IsEnabledCheckboxColumn = !_userProfileService.IsSharedProfile(_userProfile);

        EventAggregator
             .GetEvent<SavedUserProfileEvent>()
             .Subscribe(args => OnUserProfileSaved(args.UserProfile));
    }
    [RelayCommand]
    private void OnShowViewProfileSidePanel()
    {
        //_viewProfileWindowService.OpenWindow(UserProfile);
    }

    [RelayCommand]
    private void OpenOutReach()
    {
        IsOpenPopup = !IsOpenPopup;
    }

    [RelayCommand]
    private void OnOpenOutReachLink()
    {
        EventAggregator
            .GetEvent<OutReachLinksOpenEvent>()
            .Publish(new OutReachEventArgs(_userProfile));
    }

    [RelayCommand]
    private void OnOpenOutRssReach()
    {
        EventAggregator
            .GetEvent<OutReachOpenEvent>()
            .Publish(new OutReachEventArgs(_userProfile));
    }

    private void OnUserProfileSaved(IUserProfile userProfile)
    {
        if (userProfile.Id != UserProfile.Id)
        {
            return;
        }
        //TODO: ?? RaiseAllPropertiesChanged();
    }
    [RelayCommand]
    private void Favorite()
    {
        if (IsFavorite)
            FavoriteUserProfile();
        else
            UnfavoriteUserProfile();
    }

   
    private void FavoriteUserProfile()
    {
        EventAggregator
            .GetEvent<FavoriteUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }
    private void UnfavoriteUserProfile()
    {
        EventAggregator
            .GetEvent<UnfavoriteUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }
    [RelayCommand]
    private void DeleteUserProfile()
    {
        ContentDialogService.ShowContentDialogAsync(
            content: "Are you sure you want to delete this profile?",
            title: "Delete User Profile",
            action: () =>
            {
                EventAggregator
                .GetEvent<DeleteUserProfileEvent>()
                .Publish(new UserProfileEventArgs(_userProfile));
            });
    }
    [RelayCommand]
    private void OpenUserProfile()
    {
        OpenMenu();
        EventAggregator
            .GetEvent<OpenUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));
    }
    [RelayCommand]
    private void OpenUserBrowser()
    {
        EventAggregator
            .GetEvent<OpenUserBrowserEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));
    }
    [RelayCommand]
    private void OpenUserRss()
    {
        EventAggregator
           .GetEvent<OpenUserContentDiscovereyEvent>()
           .Publish(new OpenUserContentDiscovereyEventArgs(UserProfile));

        EventAggregator
            .GetEvent<OpenContentDiscovereyTabEvent>()
            .Publish(new OpenContentDiscovereyTabEventArgs(ContentDiscovereyTabs.RSS));
    }
    [RelayCommand]
    private void OpenFirefox()
    {
        OpenSystemBrowser(SystemBrowserType.Firefox);
    }
    [RelayCommand]
    private void OpenChrome()
    {
        OpenSystemBrowser(SystemBrowserType.Chrome);
    }
    [RelayCommand]
    private void OpenBrave()
    {
        OpenSystemBrowser(SystemBrowserType.Brave);
    }
    [RelayCommand]
    private void OpenSystemBrowser(SystemBrowserType browserType)
    {
        var args = new UserProfileSystemBrowserEventArgs(
            _userProfile, browserType);

        EventAggregator
            .GetEvent<OpenUserSystemBrowserEvent>()
            .Publish(args);
    }
    [RelayCommand]
    private void OpenMenu()
    {
        IsOpenMenuPopup = !IsOpenMenuPopup;
    }
    [RelayCommand]
    private void OpenSharingOptions()
    {
        _shareUserProfilePopupService.ShowPopup(_userProfile);
    }


    public IUserProfile UserProfile => _userProfile;

    public bool IsFavorite => _userProfile?.IsFavourite ?? false;
    public bool IsSharedProfile => _userProfileService.IsSharedProfile(UserProfile);

    public string SubTitle => "Profiles";
    public bool HasMultiOptions => true;

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
                    .Publish(new SelectedUserProfileEventArgs(_userProfile, _isSelected));
            }
        }
    }

    public bool IsShowCheckboxColumn { get; }
    public bool IsEnabledCheckboxColumn { get; }

    private bool _isOpenPopup;
    public bool IsOpenPopup
    {
        get => _isOpenPopup;
        set => SetProperty(ref _isOpenPopup, value);
    }
    private bool _isOpenMenuPopup;
    public bool IsOpenMenuPopup
    {
        get => _isOpenMenuPopup;
        set => SetProperty(ref _isOpenMenuPopup, value);
    }
    public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];

    public bool IsDeleteProfileBtnVisible => !IsSharedProfile && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);

    public bool IsOutreachBtnEnabled => !IsSharedProfile || _userProfile.HasPermission(PermissionNames.Pages_Outreach);

    public bool IsRssBtnEnabled => !IsSharedProfile || _userProfile.HasPermission(PermissionNames.Pages_RSS);
}