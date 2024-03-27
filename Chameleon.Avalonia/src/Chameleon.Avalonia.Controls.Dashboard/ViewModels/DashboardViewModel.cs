using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Util;
using Chameleon.CT.Common.Base;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public partial class DashboardViewModel
       : PageViewModelBase
       , IDashboardViewModel
{                
    private const string _pageTitle = "Dashboard";

    //private readonly IAuthSession _authSession;
    //private readonly IEventAggregator EventAggregator;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IShareUserProfilePopupService _shareUserProfilePopupService;
    private readonly IApplicationUser _applicationUser;
    private readonly IUserAssistantService _userAssistantService;


    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollection<IUserProfileFolder, FolderViewModel> _folderMapping;

    [ObservableProperty]
    private bool isSyncChangesBtnVisible;
    //public bool IsSyncChangesBtnVisible => _applicationUser.IsAssistant || HasAssistants();
    public DashboardViewModel(
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService,
        IShareUserProfilePopupService shareUserProfilePopupService,
        IApplicationUser applicationUser,
        IUserAssistantService userAssistantService)
    {
        Title = _pageTitle;

        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;
        _shareUserProfilePopupService = shareUserProfilePopupService;
        _applicationUser = applicationUser;
        _userAssistantService = userAssistantService;

        //EventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .SubscribeOnce(OnAuthenticated);

        EventAggregator
           .GetEvent<DeleteUserProfileEvent>()
           .Subscribe(OnUpdateViewModel);

        EventAggregator
            .GetEvent<FavoriteUserProfileEvent>()
            .Subscribe(OnUpdateViewModel);

        EventAggregator
            .GetEvent<UnfavoriteUserProfileEvent>()
            .Subscribe(OnUpdateViewModel);

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Subscribe(OnUpdateFavoriteFolders);

        EventAggregator
            .GetEvent<SavedUserProfileEvent>()
            .Subscribe(OnUpdateViewModel);

        EventAggregator
            .GetEvent<SavedUserAssistantEvent>()
            .Subscribe(async(args) => await CheckHasAssistantsAsync());

        EventAggregator
            .GetEvent<DeletedUserAssistantEvent>()
            .Subscribe(async (args) => await CheckHasAssistantsAsync());

        EventAggregator
            .GetEvent<UpdateStaleDataEvent>()
            .Subscribe(LoadAsync);

    }
    public override async Task InitAsync(object? param)
    {
        if(Loaded)
            return; 

        await base.InitAsync(param);

        IsWaiting = true;

        await LoadUserProfileViewModels();
        await LoadUserProfileFolderViewModels();
        await CheckHasAssistantsAsync();

        IsWaiting = false;
    }
    //public void OnAuthenticated()
    //{
    //    IsWaiting = true;

    //    DispatcherService.InvokeOnUiThreadAsync(
    //        () =>
    //        {
    //            LoadAsync();
    //            SyncBtnVisibilityChange();
    //        },
    //        _ => IsWaiting = false
    //        );
    //}
    private void LoadAsync()
    {
        //LoadUserProfileViewModels();
        //LoadUserProfileFolderViewModels();
    }

    private async Task LoadUserProfileViewModels()
    {
        ViewModels?.Clear();

        var userProfiles = await _userProfileService.GetAllAsync();

        _mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(
            userProfiles, profile => new UserProfileViewModel(
                    _userProfileService,
                    profile,
                    _shareUserProfilePopupService,
                    _applicationUser,
                    false
                )
            );

        OnPropertyChanged(nameof(ViewModels));
    }

    private async Task LoadUserProfileFolderViewModels()
    {
        var ascending = FolderViewModels?.Ascending ?? true;
        FolderViewModels?.Clear();

        var folders = await _userProfileFolderService.GetAllAsync();

        _folderMapping = new ObservableCollection<IUserProfileFolder, FolderViewModel>(
            folders, folder => new FolderViewModel(folder, _userProfileService, _userProfileFolderService));

        if (FolderViewModels != null)
        {
            FolderViewModels.Ascending = ascending;
        }

        OnPropertyChanged(nameof(FolderViewModels));
    }

    private async void OnUpdateFavoriteFolders()
    {
        await LoadUserProfileFolderViewModels();
    }

    private void OnUpdateViewModel(UserProfileEventArgs args)
    {
        OnPropertyChanged(nameof(HasNoItems));
    }

    private bool _isWaiting = true;
    public bool IsWaiting
    {
        get => _isWaiting;
        set => SetProperty(ref _isWaiting, value);
    }

    private ObservableCollectionView<UserProfileViewModel> _viewModels;
    public ObservableCollectionView<UserProfileViewModel> ViewModels
    {
        get
        {
            if ((_viewModels == null || _viewModels.Count == 0) && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<UserProfileViewModel>(_mapping)
                {
                    TrackItemChanges = true,
                    Order = profile => profile.Title
                };
            }

            if (_viewModels != null)
            {
                _viewModels.Filter = profile => FilterProfiles(profile.UserProfile);

                OnPropertyChanged(nameof(HasNoItems));

                if(_viewModels.Count > 0)
                {
                    SelectedProfile = _viewModels[0].UserProfile;
                }
            }

            return _viewModels;
        }
    }

    private bool FilterProfiles(IUserProfile profile)
    {
        return string.IsNullOrEmpty(_searchText) ? profile.IsFavourite : SearchResult(profile.Title, _searchText);
    }

    private bool FilterFolders(IUserProfileFolder folder)
    {
        return string.IsNullOrEmpty(_searchText) ? folder.IsFavorite : SearchResult(folder.Title, _searchText);
    }

    private bool SearchResult(string title, string searchText)
    {
        return title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase);
    }

    private ObservableCollectionView<FolderViewModel> _folderViewModels;
    public ObservableCollectionView<FolderViewModel> FolderViewModels
    {
        get
        {
            if ((_folderViewModels == null || _folderViewModels.Count == 0) && _folderMapping != null)
            {
                _folderViewModels = new ObservableCollectionView<FolderViewModel>(_folderMapping)
                {
                    TrackItemChanges = true,
                    Order = folder => folder.Title
                };
            }

            if (_folderViewModels != null)
            {
                _folderViewModels.Filter = folder => FilterFolders(folder.Folder);

                FoldersCount = _folderViewModels.Count;
                OnPropertyChanged(nameof(FoldersCount));
            }

            return _folderViewModels;
        }
    }

    private int _foldersCount;
    public int FoldersCount
    {
        get => _foldersCount;
        set
        {
            if (SetProperty(ref _foldersCount, value))
            {
                OnPropertyChanged(nameof(HasNoFolderItems));
            }
        }
    }

   //private BreadcrumbsViewModel _breadcrumbsViewModel;
   //public BreadcrumbsViewModel BreadcrumbsViewModel
   //{
   //    get
   //    {
   //        if (_breadcrumbsViewModel == null)
   //        {
   //            _breadcrumbsViewModel = new BreadcrumbsViewModel();
   //
   //            var root = new BreadcrumbViewModel
   //            {
   //                Title = _pageTitle
   //            };
   //
   //            _breadcrumbsViewModel.Breadcrumbs.Add(root);
   //        }
   //
   //        return _breadcrumbsViewModel;
   //    }
   //}

    public bool HasNoItems => _viewModels?.Count == 0;

    public bool HasNoFolderItems => FoldersCount == 0;

    public ListSortDirection[] Sorts { get; } = (ListSortDirection[])Enum.GetValues(typeof(ListSortDirection));

    private ListSortDirection _sortSelected = ListSortDirection.Ascending;
    public ListSortDirection SortSelected
    {
        get => _sortSelected;
        set
        {
            if (SetProperty(ref _sortSelected, value))
            {
                ViewModels.Ascending = value == ListSortDirection.Ascending;
            }
        }
    }

    private ListSortDirection _folderSortSelected = ListSortDirection.Ascending;
    public ListSortDirection FolderSortSelected
    {
        get => _folderSortSelected;
        set
        {
            if (SetProperty(ref _folderSortSelected, value))
            {
                FolderViewModels.Ascending = value == ListSortDirection.Ascending;
            }
        }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                OnPropertyChanged(nameof(ViewModels));
                OnPropertyChanged(nameof(FolderViewModels));
                OnPropertyChanged(nameof(NoSearchResultsInFavorite));
            }
        }
    }

    public bool NoSearchResultsInFavorite => string.IsNullOrEmpty(SearchText);

    [RelayCommand]
    private void SyncChanges()
    {
        EventAggregator
            .GetEvent<SyncChangesEvent>()
            .Publish();
    }

    //private void SyncBtnVisibilityChange()
    //{
    //    OnPropertyChanged(nameof(IsSyncChangesBtnVisible));
    //}

    private async Task CheckHasAssistantsAsync()
    {
        var assists = await _userAssistantService.GetAsync();
        IsSyncChangesBtnVisible = _applicationUser.IsAuthenticated && assists?.Count > 0;
    }

    public IUserProfile SelectedProfile { get; set; }
}
