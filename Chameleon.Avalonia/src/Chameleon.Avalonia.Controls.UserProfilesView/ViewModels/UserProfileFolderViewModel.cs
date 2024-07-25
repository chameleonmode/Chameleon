using Chameleon.Application.Events;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
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
    private readonly UserProfileFoldersViewModel foldervm;  

    private bool _isSelected;

    [ObservableProperty]
    private bool _isFavoriteButtonVisible = true;
    [ObservableProperty]
    private bool _isRenamed;

    [ObservableProperty]
    private bool _isFavorite;

    private IList<IUserProfile> ProfilesByCurrentFolder
    {
        get
        {
            var userProfilesFromCurrentFolder = _userProfileService
                .GetAll()
                .Where(profiles => profiles.FolderId == _folder.Id)
                .ToList();

            return userProfilesFromCurrentFolder;
        }
    }

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

    public bool IsFolderNotEmpty => ProfilesByCurrentFolder.Any();
    public IUserProfileFolder UserProfileFolder => _folder;
    public IApplicationUser CurrentUser => _currentUser;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(_folder);
    public bool IsContextMenuItemEnabled => !CurrentUser.IsAssistant;

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
        IsFavorite = folder.IsFavorite;

        EventAggregator.Sub<UpdateUserProfileFolderEvent, UserProfileFolderEventArgs>(a =>
        {
            if (a.UserProfileFolder.Id == _folder.Id)
            {
                IsFavorite = _folder.IsFavorite = a.UserProfileFolder.IsFavorite;
            }
        });
    }

   [RelayCommand]
    public async Task Open()
    {
        await foldervm.OnNavigatingTo(UserProfileFolder);
        IsSelected = true;
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
    private async Task Delete()
    {
        if (await MesageBoxHelper.ShowAsync("Delete Folder",
            $"Are you sure you want to delete {UserProfileFolder.Title} folder? This will not affect individual profiles within the folder.",
            ContentDialogButtons.YesNo,
            "DeleteLines"))
        {

            await ContainerServiceHelper.Resolve<IUserProfileFolderEventHandler>().DeleteFolder(_folder);
            await foldervm.AllProfiles.Open();
        }
    }

    [RelayCommand]
    private void StartRename()
    {
        Title = _folder.Title;
        IsRenamed = true;
    }

    [RelayCommand]
    private void ChangeProxies()
    {
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
}
