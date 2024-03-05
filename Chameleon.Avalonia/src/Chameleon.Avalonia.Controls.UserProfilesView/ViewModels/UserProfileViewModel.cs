using Chameleon.Authorization;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Avalonia.Prism.Module.MessageBox.ViewModels;
using Chameleon.Interfaces.App.ContentDiscoverey;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Drawing;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public class UserProfileViewModel : ViewModelBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfile _userProfile;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IShareUserProfilePopupService _shareUserProfilePopupService;
    private readonly IApplicationUser _applicationUser;

    public UserProfileViewModel(
        IUserProfileService userProfileService,
        IUserProfile userProfile,
        IEventAggregator eventAggregator,
        IMessageBoxService messageBoxService,
        IShareUserProfilePopupService shareUserProfilePopupService,
        IApplicationUser applicationUser,
        bool isShowCheckboxColumn = true
        )
    {
        _userProfileService = userProfileService;
        _userProfile = userProfile;
        _eventAggregator = eventAggregator;
        _messageBoxService = messageBoxService;
        _shareUserProfilePopupService = shareUserProfilePopupService;
        _applicationUser = applicationUser;
        IsShowCheckboxColumn = isShowCheckboxColumn && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
        IsEnabledCheckboxColumn = !_userProfileService.IsSharedProfile(_userProfile);

        OpenFirefoxCommand = new DelegateCommand(OpenFirefox);
        OpenChromeCommand = new DelegateCommand(OpenChrome);
        OpenBraveCommand = new DelegateCommand(OpenBrave);
        OpenDetailsCommand = new DelegateCommand(OpenUserProfile);
        OpenBrowserCommand = new DelegateCommand(OpenUserBrowser);
        OpenRssCommand = new DelegateCommand(OpenUserRss);
        DeleteCommand = new DelegateCommand(DeleteUserProfile);
        UnfavoriteCommand = new DelegateCommand(UnfavoriteUserProfile);
        FavouriteCommand = new DelegateCommand(FavoriteUserProfile);
        OpenOutReachRssCommand = new DelegateCommand(OnOpenOutRssReach);
        OpenOutReachLinkCommand = new DelegateCommand(OnOpenOutReachLink);
        OpenOutReachCommand = new DelegateCommand(OpenOutReach);
        OpenMenuCommand = new DelegateCommand(OpenMenu);
        OpenSharingOptionsCommand = new DelegateCommand(OpenSharingOptions);

        _eventAggregator
             .GetEvent<SavedUserProfileEvent>()
             .Subscribe(args => OnUserProfileSaved(args.UserProfile));
    }

    public DelegateCommand OpenOutReachCommand { get; }
    private void OpenOutReach()
    {
        IsOpenPopup = !IsOpenPopup;
    }

    public DelegateCommand OpenOutReachLinkCommand { get; }
    private void OnOpenOutReachLink()
    {
        _eventAggregator
            .GetEvent<OutReachLinksOpenEvent>()
            .Publish(new OutReachEventArgs(_userProfile));
    }

    public DelegateCommand OpenOutReachRssCommand { get; }

    private void OnOpenOutRssReach()
    {
        _eventAggregator
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

    private void FavoriteUserProfile()
    {
        _eventAggregator
            .GetEvent<FavoriteUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));

        _eventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }

    private void UnfavoriteUserProfile()
    {
        _eventAggregator
            .GetEvent<UnfavoriteUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));

        _eventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }

    private void DeleteUserProfile()
    {
        var messageBoxOptions = new MessageBoxOptions
        {
            Title = "Delete User Profile",
            Text = "Are you sure you want to delete this profile?",
            Buttons = MessageBoxButton.YesNo,
            Icon = SystemIcons.Question,
            //TODO: ? DefaultButton = ButtonResult.No,
            ContentButtons = new MessageBoxContentButtonsViewModel { ContentOkButton = "Yes" }
        };

        ButtonResult result = (ButtonResult)_messageBoxService.ShowDialog(messageBoxOptions);

        if (result == ButtonResult.OK)
        {
            _eventAggregator
                .GetEvent<DeleteUserProfileEvent>()
                .Publish(new UserProfileEventArgs(_userProfile));
        }
    }

    private void OpenUserProfile()
    {
        _eventAggregator
            .GetEvent<OpenUserProfileEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));
    }

    private void OpenUserBrowser()
    {
        _eventAggregator
            .GetEvent<OpenUserBrowserEvent>()
            .Publish(new UserProfileEventArgs(_userProfile));
    }

    private void OpenUserRss()
    {
        _eventAggregator
           .GetEvent<OpenUserContentDiscovereyEvent>()
           .Publish(new OpenUserContentDiscovereyEventArgs(UserProfile));

        _eventAggregator
            .GetEvent<OpenContentDiscovereyTabEvent>()
            .Publish(new OpenContentDiscovereyTabEventArgs(ContentDiscovereyTabs.RSS));
    }

    private void OpenFirefox()
    {
        OpenSystemBrowser(SystemBrowserType.Firefox);
    }

    private void OpenChrome()
    {
        OpenSystemBrowser(SystemBrowserType.Chrome);
    }

    private void OpenBrave()
    {
        OpenSystemBrowser(SystemBrowserType.Brave);
    }

    private void OpenSystemBrowser(SystemBrowserType browserType)
    {
        var args = new UserProfileSystemBrowserEventArgs(
            _userProfile, browserType);

        _eventAggregator
            .GetEvent<OpenUserSystemBrowserEvent>()
            .Publish(args);
    }

    private void OpenMenu()
    {
        IsOpenMenuPopup = !IsOpenMenuPopup;
    }

    private void OpenSharingOptions()
    {
        _shareUserProfilePopupService.ShowPopup(_userProfile);
    }


    public IUserProfile UserProfile => _userProfile;
    public DelegateCommand OpenChromeCommand { get; }
    public DelegateCommand OpenBraveCommand { get; }
    public DelegateCommand OpenFirefoxCommand { get; }
    public DelegateCommand OpenBrowserCommand { get; }
    public DelegateCommand OpenRssCommand { get; }
    public DelegateCommand OpenDetailsCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand UnfavoriteCommand { get; }
    public DelegateCommand FavouriteCommand { get; }
    public DelegateCommand OpenMenuCommand { get; }
    public DelegateCommand OpenSharingOptionsCommand { get; }

    public bool IsFavorite => _userProfile?.IsFavourite ?? false;
    public string Title => _userProfile?.Title;
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
                _eventAggregator
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