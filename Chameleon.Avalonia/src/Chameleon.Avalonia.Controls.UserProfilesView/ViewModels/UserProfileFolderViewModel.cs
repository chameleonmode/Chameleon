using Chameleon.Application.Events;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileFolderViewModel : SubPageViewModelBase
{
    private readonly IApplicationUser _currentUser;
    private readonly IUserProfileFolder _folder;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IUserProfileService _userProfileService;

    UserProfileFoldersViewModel foldervm;

    [ObservableProperty]
    private bool _isFavoriteButtonVisible = true;

    private IList<IUserProfile> _selectedUserProfiles = new List<IUserProfile>();
    public IList<IUserProfile> SelectedUserProfiles
    {
        get => _selectedUserProfiles;
        set => SetProperty(ref _selectedUserProfiles, value);
    }

    public bool IsFolderNotEmpty => GetProfilesByCurrentFolder().Any();

    public UserProfileFolderViewModel(
        IApplicationUser currentUser,
        IUserProfileFolder folder,
        IUserProfileFolderService userProfileFolderService,
        UserProfileFoldersViewModel f,
        IUserProfileService userProfileService)
    {
        _currentUser = currentUser;
        _userProfileService = userProfileService;
        _folder = folder;
        _userProfileFolderService = userProfileFolderService;
        foldervm = f;

        EventAggregator
            .GetEvent<SavedUserProfileFolderEvent>()
            .Subscribe(OnFolderSaved);

        //EventAggregator
        //    .GetEvent<OpenUserProfileFolderEvent>()
        //    .Subscribe(SetSelected);

        EventAggregator
            .GetEvent<UpdateUserProfileFolderEvent>()
            .Subscribe(SetSelected);

        IsFavorite = folder.IsFavorite;
    }

    public IUserProfileFolder UserProfileFolder => _folder;

   [RelayCommand]
    public void Open()
    {
        foldervm.OnNavigatingTo(UserProfileFolder);
        //foldervm.OnNavigatingTo(null);
        IsSelected = true;
        //IsSelected = true;
        //ContainerServiceHelper.Resolve<IUserProfilesViewModel>().Open(UserProfileFolder);
        //EventAggregator
        //    .GetEvent<OpenUserProfileFolderEvent>()
        //    .Publish(new UserProfileFolderEventArgs(_folder));
    }

    [RelayCommand]
    private void SetFavorite()
    {
        IsFavorite = !IsFavorite;

        UserProfileFolder.IsFavorite = IsFavorite;

        _userProfileFolderService.Save(_folder);

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Publish(new ChangeProfilesInFavoriteFolderEventArgs(UserProfileFolder.Id)); 
        
        OnPropertyChanged(nameof(UserProfileFolder));
    }

    [RelayCommand]
    private async Task OpenAutomation()
    {
        var userProfilesToApply = GetProfilesByCurrentFolder();

        var result = await ContentDialogService
           .ShowAsync<ISelectAutomationPopupView, ISelectAutomationPopupViewModel>(viewModel =>
           {
               viewModel.Title = "Select Automation";
               viewModel.UserProfiles = userProfilesToApply;
           });
    }

    private IList<IUserProfile> GetProfilesByCurrentFolder()
    {
        var userProfilesFromCurrentFolder = _userProfileService
            .GetAll()
            .Where(profiles => profiles.FolderId == _folder.Id)
            .ToList();

        return userProfilesFromCurrentFolder;
    }

    private void SetSelected(UserProfileFolderEventArgs args)
    {
        IsSelected = args.UserProfileFolder.Id == _folder.Id;
    }

    private void OnFolderSaved(UserProfileFolderEventArgs args)
    {
        if (args.UserProfileFolder != _folder)
        {
            return;
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        IsOpenMenuPopup = false;

        if (await MesageBoxHelper.ShowAsync("Delete Folder",
            $"Are you sure you want to delete {UserProfileFolder.Title} folder? This will not affect individual profiles within the folder.",
            ContentDialogButtons.YesNo,
            "DeleteLines"))
        {

            await ContainerServiceHelper.Resolve<IUserProfileFolderEventHandler>().DeleteFolder(_folder);
            foldervm.AllProfiles.Open();
        }
           // EventAggregator.Publish<DeleteUserProfileFolderEvent, UserProfileFolderEventArgs>(new UserProfileFolderEventArgs(_folder));
                //.GetEvent<DeleteUserProfileFolderEvent>()
                //.Publish(new UserProfileFolderEventArgs(_folder));
    }

    [RelayCommand]
    private void OpenMenu()
    {
        IsOpenMenuPopup = !IsOpenMenuPopup;
    }

    private bool _isOpenMenuPopup;
    public bool IsOpenMenuPopup
    {
        get => _isOpenMenuPopup;
        set => SetProperty(ref _isOpenMenuPopup, value);
    }

   [RelayCommand]
    private void StartRename()
    {
        Title = _folder.Title;
        IsOpenMenuPopup = false;
        IsRenamed = true;
    }

    [RelayCommand]
    private void ChangeProxies()
    {
        //EventAggregator
        //    .GetEvent<OpenChangeProxiesEvent>()
        //    .Publish(new OpenChangeProxiesEventArgs(_folder.Id));

        NavigationService.NavigateToType(typeof(IUserProxySettingsView), UserProfileFolder);
    }

    [RelayCommand]
    private void SaveRename()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return;
        }

        var orignalTitle = _folder.Title;
        try
        {
            _folder.Title = Title;
            _userProfileFolderService.Save(_folder);

            EventAggregator
                .GetEvent<RenameFolderEvent>()
                .Publish(new RenameFolderEventArgs(_folder.Id, _folder.Title));
        }
        catch
        {
            _folder.Title = orignalTitle;
        }

        Title = _folder.Title;

        IsRenamed = false;

        EventAggregator
           .GetEvent<SyncChangesEvent>()
           .Publish();
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetProperty(ref _isSelected, value);
            if (value == false)
            {
                IsRenamed = false;
            }
            else
            {
                foldervm.SelectedFolder = this;
            }
        }
    }

    private bool _isRenamed;
    public bool IsRenamed
    {
        get => _isRenamed;
        set => SetProperty(ref _isRenamed, value);
    }

    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    public IApplicationUser CurrentUser => _currentUser;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(_folder);
    public bool IsContextMenuItemEnabled => !CurrentUser.IsAssistant;
    
}
