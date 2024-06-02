using Avalonia.Controls;
using Chameleon.Application.Events;
using Chameleon.Common.Helpers;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfileFolderViewModel : SubPageViewModelBase
{
    private readonly IApplicationUser _currentUser;
    private readonly IUserProfileFolder _folder;
    private readonly IUserProfileFolderService _userProfileFolderService;

    UserProfileFoldersViewModel foldervm;

   [ObservableProperty]
    private bool _isFavoriteButtonVisible = true;

    public UserProfileFolderViewModel(
        IApplicationUser currentUser,
        IUserProfileFolder folder,
        IUserProfileFolderService userProfileFolderService,
        UserProfileFoldersViewModel f)
    {
        _currentUser = currentUser;
        _folder = folder;
        _userProfileFolderService = userProfileFolderService;
        foldervm = f;
        IsFavorite = folder.IsFavorite;

        EventAggregator.Sub<UpdateUserProfileFolderEvent, UserProfileFolderEventArgs>((a) =>
        {
            if (a.UserProfileFolder.Id == _folder.Id)
            {
                IsFavorite = _folder.IsFavorite = a.UserProfileFolder.IsFavorite;
            }
        });
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
    private async Task Delete()
    {
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
    private void StartRename()
    {
        Title = _folder.Title;
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
    
    [ObservableProperty]
    private bool _isRenamed;

    [ObservableProperty]
    private bool _isFavorite;

    public IApplicationUser CurrentUser => _currentUser;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(_folder);
    public bool IsContextMenuItemEnabled => !CurrentUser.IsAssistant;
    
}
