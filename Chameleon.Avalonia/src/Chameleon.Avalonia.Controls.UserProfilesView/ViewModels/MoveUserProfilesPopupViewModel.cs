using Avalonia.Controls;
using Chameleon.CT.Common.Base;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class MoveUserProfilesPopupViewModel : ObservableObjectBase, 
    IMoveUserProfilesPopupViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userFolderService;

    public MoveUserProfilesPopupViewModel(
        IUserProfileService userProfileService,
        IUserProfileFolderService userFolderService
        )
    {
        _userProfileService = userProfileService;
        _userFolderService = userFolderService;

       
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);
            Initialize();
    }
    private IList<IUserProfile> _profiles;
    public IList<IUserProfile> Profiles
    {
        get => _profiles;
        set => SetProperty(ref _profiles, value);
    }

    private IUserProfileFolder _selectedFolder;
    public IUserProfileFolder SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (SetProperty(ref _selectedFolder, value))
            {
                HasSelected = value != null;
                //SaveChangesCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private ObservableCollection<IUserProfileFolder> _folders = [];
    public ObservableCollection<IUserProfileFolder> Folders
    {
        get => _folders;
        set => SetProperty(ref _folders, value);
    }

    // public DelegateCommand<IUserProfileFolder> SelectFolderCommand { get; private set; }
    [RelayCommand]
    private void SelectFolder(IUserProfileFolder selectedFolder)
    {
        SelectedFolder = selectedFolder;
    }


    private void MoveProfilesToFolder()
    {
        if (SelectedFolder is null || !_profiles.Any())
        {
            return;
        }

        var ids = _profiles
            .Select(a => a.Id)
            .ToList();

        _userProfileService.MoveUserProfileToFolder(ids, SelectedFolder.Id);

        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Publish(new ChangeProfilesInFavoriteFolderEventArgs(SelectedFolder.Id));
    }

    private bool _hasSelected;
    public bool HasSelected
    {
        get => _hasSelected;
        set => SetProperty(ref _hasSelected, value);
    }

    private bool _listIsVisible = true;
    public bool ListIsVisible
    {
        get => _listIsVisible;
        set => SetProperty(ref _listIsVisible, value);
    }

    private void Initialize()
    {
        Folders.Clear();
        Folders.AddRange(_userFolderService.GetAll());
        OnPropertyChanged(nameof(Folders));
    }

    public void OnDialogClosing(IContentDialogResult result)
    {
        if (result == IContentDialogResult.Primary)
            MoveProfilesToFolder();
    }
}