using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public partial class DashboardViewModel
       : PageViewModelBase
       , IDashboardViewModel
{
    private const string _pageTitle = "Dashboard";

    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IApplicationUser _applicationUser;
    private readonly IUserAssistantService _userAssistantService;


    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollection<IUserProfileFolder, FolderViewModel> _folderMapping;

    [ObservableProperty]
    private bool isSyncChangesBtnVisible;

    public DashboardViewModel(
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService,
        IApplicationUser applicationUser,
        IUserAssistantService userAssistantService)
    {
        Title = _pageTitle;

        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;
        _applicationUser = applicationUser;
        _userAssistantService = userAssistantService;
        //_systemBrowserManager = systemBrowserManager;

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
            .GetEvent<SavedUserProfileEvent>()
            .Subscribe(OnUpdateViewModel);

        EventAggregator
            .GetEvent<UpdateFavoriteFolderEvent>()
            .Subscribe(OnUpdateFavoriteFolders);

        EventAggregator
            .GetEvent<SavedUserAssistantEvent>()
            .Subscribe(async (args) => await CheckHasAssistantsAsync());

        EventAggregator
            .GetEvent<DeletedUserAssistantEvent>()
            .Subscribe(async (args) => await CheckHasAssistantsAsync());

        EventAggregator
            .GetEvent<UpdateStaleDataEvent>()
            .Subscribe(LoadAsync);
    }
    public override async Task InitAsync(object? param)
    {
        if (!Loaded)
        {
            await base.InitAsync(param);

            IsWaiting = true;

            await LoadUserProfileViewModels();
            await LoadUserProfileFolderViewModels();
            await CheckHasAssistantsAsync();

            IsWaiting = false;
            BuildSearchTerms();
        }

    }

    private void LoadAsync()
    {
        //LoadUserProfileViewModels();
        //LoadUserProfileFolderViewModels();
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
            }

            return _viewModels;
        }
    }

    private async Task LoadUserProfileViewModels()
    {
        ViewModels?.Clear();

        var userProfiles = await _userProfileService.GetAllAsync();

        _mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(
            userProfiles, profile => new UserProfileViewModel(
                    _userProfileService,
                    profile as UserProfile,
                    _applicationUser,
                    false
                )
            );
        _mapping.CollectionChanged += Mapping_CollectionChanged;

        OnPropertyChanged(nameof(ViewModels));
    }

    private void Mapping_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        BuildSearchTerms();
    }

    private async Task LoadUserProfileFolderViewModels()
    {
        FolderViewModels?.Clear();

        var folders = await _userProfileFolderService.GetAllAsync();

        _folderMapping = new ObservableCollection<IUserProfileFolder, FolderViewModel>(
            folders, folder => new FolderViewModel(folder, _userProfileService, _userProfileFolderService));
        _folderMapping.CollectionChanged += FolderMapping_CollectionChanged;

        OnPropertyChanged(nameof(FolderViewModels));
    }

    private void FolderMapping_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        BuildSearchTerms();
    }

    private void OnUpdateFavoriteFolders()
    {
        OnPropertyChanged(nameof(FolderViewModels));
    }


    private void OnUpdateViewModel(UserProfileEventArgs args)
    {
        OnPropertyChanged(nameof(FolderViewModels));
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasNoItems));
    }

    private bool _isWaiting = true;
    public bool IsWaiting
    {
        get => _isWaiting;
        set => SetProperty(ref _isWaiting, value);
    }

    private bool FilterProfiles(IUserProfile profile)
    {
        return string.IsNullOrEmpty(_searchText) ? profile.IsFavourite : SearchResult(profile.Title, _searchText);
    }

    private bool FilterFolders(IUserProfileFolder folder)
    {
        return string.IsNullOrEmpty(_searchText) ? folder.IsFavorite : SearchResult(folder.Title, _searchText);
    }

    private static bool SearchResult(string? title, string searchText)
    {
        return title?.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) ?? false;
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

    private async Task CheckHasAssistantsAsync()
    {
        var assists = await _userAssistantService.GetAsync();
        IsSyncChangesBtnVisible = _applicationUser.IsAuthenticated && assists?.Count > 0;
    }

    public IUserProfile SelectedProfile { get { return ViewModels[0].UserProfile; } set { } }

    public void BuildSearchTerms()
    {
        List<MainAppSearchItem> items = [];

        foreach (var item in _mapping)
            items.Add(new()
            {
                Header = item.Title,
                Namespace = "Profile",
                ViewModel = item.UserProfile,
                PageType = this.GetType()
            });

        foreach (var item in _folderMapping)
            items.Add(new()
            {
                Header = item.Title,
                Namespace = "Profiles " + item.ProfilesCount,
                ViewModel = item.Folder,
                PageType = this.GetType()
            });

        MVVM?.BuildSearchTerms(items);
    }
}
