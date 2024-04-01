using Chameleon.Av.Fluent.Common.Services;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Events;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Linq;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public partial class FolderViewModel : SubPageViewModelBase
{
    private readonly IUserProfileFolder _folder;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly INavigationService _navigationService;
    public FolderViewModel(
        IUserProfileFolder folder,
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService,
        INavigationService nav)
    {
        _folder = folder;
        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;
        _navigationService = nav;

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

        var folder = _userProfileFolderService.Get(folderId);
        ProfilesCount = _userProfileService.GetAll().Count(a => a.FolderId == folderId);
        OnPropertyChanged(nameof(ProfilesCount));
    }

    [RelayCommand]
    private void ViewGroup()
    {
        _navigationService.NavigateToType(typeof(IProjectsView), _folder);
        //EventAggregator
        //    .GetEvent<OpenUserProfilesViewEvent>()
        //    .Publish();

        //EventAggregator
        //    .GetEvent<OpenUserProfileFolderEvent>()
        //    .Publish(new UserProfileFolderEventArgs(_folder));
    }

    [RelayCommand]
    private void SetFavoriteFolder()
    {
        IsFavorite = !IsFavorite;
        OnPropertyChanged(nameof(IsFavorite));

        _folder.IsFavorite = IsFavorite;
        _userProfileFolderService.Save(_folder);

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }

    private int _id;
    public int Id
    {
        get
        {
            if (_id == 0)
            {
                _id = _folder.Id;
            }
            return _id;
        }
        set => SetProperty(ref _id, value);
    }


    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    private int _profilesCount;
    public int ProfilesCount
    {
        get => _profilesCount;
        set => SetProperty(ref _profilesCount, value);
    }

    public IUserProfileFolder Folder => _folder;
}
