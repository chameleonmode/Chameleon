using AutoMapper;
using Avalonia.Controls;
using Chameleon.Authorization;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.UserSettings;
using Chameleon.Interfaces.App.ContentDiscoverey;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Drawing;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileViewModel : SubPageViewModelBase , IUserProfileViewModelBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IApplicationUser _applicationUser;
   // private readonly ISystemBrowserManager _systemBrowserManager;

    [ObservableProperty]
    private  IUserProfile _userProfile;

    public UserProfileViewModel(
        IUserProfileService userProfileService,
        IUserProfile userProfile,
        IApplicationUser applicationUser,
       // ISystemBrowserManager systemBrowserManager,
        bool isShowCheckboxColumn = true
        )
    {
      //  _systemBrowserManager = systemBrowserManager;
        _userProfileService = userProfileService;
        _userProfile = userProfile;
        _applicationUser = applicationUser;   

        Title = _userProfile.Title;

        IsShowCheckboxColumn = isShowCheckboxColumn && _applicationUser.HasPemission(PermissionNames.Pages_DeleteProfiles);
        IsEnabledCheckboxColumn = !_userProfileService.IsSharedProfile(_userProfile);

        EventAggregator
             .GetEvent<SavedUserProfileEvent>()
             .Subscribe(args => OnUserProfileSaved(args.UserProfile));

        OnPropertyChanged(nameof(UserProfile));
    }
    [RelayCommand]
    private void ShowViewProfile()
    {
        ContainerServiceHelper.Resolve<IWindowDialogService>().ShowTopmost<IUserProfileSidePanelView, IUserProfileSidePanelViewModel>(vm =>
        {
            vm.UserProfile = UserProfile;
        });
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
        OnPropertyChanged(string.Empty);
    }
    [RelayCommand]
    private void Favorite()
    {
        if (!IsFavorite)
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
        OpenUserProfile();
    }
    [RelayCommand]
    private void OpenUserProfile()
    {
        OpenMenu();
        NavigationService.NavigateToType(typeof(IUserProfileIdentityView), UserProfile);
        //EventAggregator
        //    .GetEvent<OpenUserProfileEvent>()
        //    .Publish(new UserProfileEventArgs(UserProfile));
    }
    [RelayCommand]
    private void OpenUserBrowser()
    {
        EventAggregator
            .GetEvent<OpenUserBrowserEvent>()
            .Publish(new UserProfileEventArgs(UserProfile));
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
        //await MesageBoxHelper.ShowAsync("",Directory.GetCurrentDirectory());
        string? uri = null;
        //TODO:
        //IUserDefaultSettingsService userDefaultsSettingsService = ContainerServiceHelper.Resolve<IUserDefaultSettingsService>();
        //var defaults = await Task.Run(()=>userDefaultsSettingsService.GetAll());
        //if (defaults.Any())
        //    uri = defaults[new Random().Next(defaults.Count)].DefaultUrl;

      var args = new UserProfileSystemBrowserEventArgs(
            UserProfile, browserType, uri);

        EventAggregator
            .GetEvent<OpenUserSystemBrowserEvent>()
            .Publish(args);
    }

    [RelayCommand]
    private void OpenMenu()
    {
        IsOpenMenuPopup = !IsOpenMenuPopup;
    }

    public bool IsFavorite => UserProfile?.IsFavourite ?? false;
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