using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles.Events;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Linq;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public partial class FolderViewModel : SubPageViewModelBase
{
    private readonly IUserProfileFolder _folder;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;

    public FolderViewModel(
        IUserProfileFolder folder,
        IEventAggregator eventAggregator,
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService)
    {
        _folder = folder;
        _eventAggregator = eventAggregator;
        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;

        Title = _folder.Title;
        IsFavorite = _folder.IsFavorite;
        ProfilesCount = _folder.ProfilesCount;

        _eventAggregator
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
    private void OnViewGroup()
    {
        _eventAggregator
            .GetEvent<OpenUserProfilesViewEvent>()
            .Publish();

        _eventAggregator
            .GetEvent<OpenUserProfileFolderEvent>()
            .Publish(new UserProfileFolderEventArgs(_folder));
    }

    [RelayCommand]
    private void OnSetFavoriteFolder()
    {
        IsFavorite = !IsFavorite;
        OnPropertyChanged(nameof(IsFavorite));

        _folder.IsFavorite = IsFavorite;
        _userProfileFolderService.Save(_folder);

        _eventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Publish();
    }

    private int _id;
    public int Id
    {
        get
        {
            if (Id == 0)
            {
                Id = _folder.Id;
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
