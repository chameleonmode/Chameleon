using Chameleon.Av.Fluent.Common.Services;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Linq;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public partial class FolderViewModel : SubPageViewModelBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private int _profilesCount;

    private IUserProfileFolder _folder;
    public IUserProfileFolder Folder { get => _folder; }

    public FolderViewModel(
        IUserProfileFolder folder,
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService)
    {
        _folder = folder;
        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;

        Title = _folder.Title;
        IsFavorite = _folder.IsFavorite;
        ProfilesCount = _folder.ProfilesCount;

        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Subscribe(args => OnChangeProfilesInFavoriteFolder(args.FolderId));
    }

    private void OnChangeProfilesInFavoriteFolder(int folderId)
    {
        if (_folder.Id != folderId)
        {
            return;
        }
        _folder = _userProfileFolderService.Get(folderId);
        ProfilesCount = _userProfileService.GetAll().Count(a => a.FolderId == folderId);          
        IsFavorite = _folder.IsFavorite;
        OnPropertyChanged(nameof(ProfilesCount));
    }

    [RelayCommand]
    private void ViewGroup()
    {
        _folder.Navigated = false;
        NavigationService?.NavigateToType(typeof(IProjectsView), _folder);
    }

    [RelayCommand]
    private void SetFavoriteFolder()
    {
        IsFavorite = !IsFavorite;

        _folder.IsFavorite = IsFavorite;
        _userProfileFolderService.Save(_folder);

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();

        EventAggregator.Push<UpdateUserProfileFolderEvent, UserProfileFolderEventArgs>(Folder);
    }
}
