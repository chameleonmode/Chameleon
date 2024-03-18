using Chameleon.Avalonia.Common.Util;
using Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;
using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Util;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using Prism.Commands;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.Dashboard.ViewModels;

public class DashboardViewModel
       : ViewModelBase
       , IDashboardViewModel
{
    private readonly IAuthSession _authSession;
    private readonly IEventAggregator _eventAggregator;
    private readonly IUserProfileService _userProfileService;
    private readonly IPrismMessageBoxService _messageBoxService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IShareUserProfilePopupService _shareUserProfilePopupService;
    private readonly IApplicationUser _applicationUser;
    private readonly IUserAssistantService _userAssistantService;

    private const string _pageTitle = "Dashboard";

    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollection<IUserProfileFolder, FolderViewModel> _folderMapping;


  public DashboardViewModel(
      IAuthSession authSession,
      IEventAggregator eventAggregator,
      IUserProfileService userProfileService,
      IPrismMessageBoxService messageBoxService,
      IUserProfileFolderService userProfileFolderService,
      IShareUserProfilePopupService shareUserProfilePopupService,
      IApplicationUser applicationUser,
      IUserAssistantService userAssistantService
      )
  {
      _authSession = authSession;
      _eventAggregator = eventAggregator;
      _userProfileService = userProfileService;
      _messageBoxService = messageBoxService;
      _userProfileFolderService = userProfileFolderService;
      _shareUserProfilePopupService = shareUserProfilePopupService;
      _applicationUser = applicationUser;
      _userAssistantService = userAssistantService;
  
      SyncChangesCommand = new DelegateCommand(SyncChanges);
  
      _eventAggregator
          .GetEvent<LoginSuccessEvent>()
          .SubscribeOnce(OnAuthenticated);
  
      _eventAggregator
         .GetEvent<DeleteUserProfileEvent>()
         .Subscribe(OnUpdateViewModel);
  
      _eventAggregator
          .GetEvent<FavoriteUserProfileEvent>()
          .Subscribe(OnUpdateViewModel);
  
      _eventAggregator
          .GetEvent<UnfavoriteUserProfileEvent>()
          .Subscribe(OnUpdateViewModel);
  
      _eventAggregator
          .GetEvent<UpdateFavoriteFolderEvent>()
          .Subscribe(OnUpdateFavoriteFolders);
  
      _eventAggregator
          .GetEvent<SavedUserProfileEvent>()
          .Subscribe(OnUpdateViewModel);
  
      _eventAggregator
          .GetEvent<SavedUserAssistantEvent>()
          .Subscribe(args => SyncBtnVisibilityChange());
  
      _eventAggregator
          .GetEvent<DeletedUserAssistantEvent>()
          .Subscribe(args => SyncBtnVisibilityChange());
  
      _eventAggregator
          .GetEvent<UpdateStaleDataEvent>()
          .Subscribe(LoadAsync);
  }

    private void OnUpdateViewModel(UserProfileEventArgs args)
    {
        RaisePropertyChanged(nameof(HasNoItems));
    }

    private bool _isWaiting = true;
    public bool IsWaiting
    {
        get => _isWaiting;
        set => SetProperty(ref _isWaiting, value);
    }

    public void OnAuthenticated()
    {
        IsWaiting = true;

        DispatcherService.InvokeOnUiThreadAsync(
            () =>
            {
                LoadAsync();
                SyncBtnVisibilityChange();
            },
            _ => IsWaiting = false
            );
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

                RaisePropertyChanged(nameof(HasNoItems));
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
                RaisePropertyChanged(nameof(FoldersCount));
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
                RaisePropertyChanged(nameof(HasNoFolderItems));
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
                RaisePropertyChanged(nameof(ViewModels));
                RaisePropertyChanged(nameof(FolderViewModels));
                RaisePropertyChanged(nameof(NoSearchResultsInFavorite));
            }
        }
    }

    public bool NoSearchResultsInFavorite => string.IsNullOrEmpty(SearchText);

    private void LoadAsync()
    {
        LoadUserProfileViewModels();
        LoadUserProfileFolderViewModels();
    }

    private void LoadUserProfileViewModels()
    {
        ViewModels?.Clear();

        var userProfiles = _userProfileService.GetAll();

        _mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(
            userProfiles, profile => new UserProfileViewModel(
                    _userProfileService,
                    profile,
                    _eventAggregator,
                    _messageBoxService,
                    _shareUserProfilePopupService,
                    _applicationUser,
                    false
                )
            );

        RaisePropertyChanged(nameof(ViewModels));
    }

    private void LoadUserProfileFolderViewModels()
    {
        var ascending = FolderViewModels?.Ascending ?? true;
        FolderViewModels?.Clear();

        var folders = _userProfileFolderService.GetAll();

        _folderMapping = new ObservableCollection<IUserProfileFolder, FolderViewModel>(
            folders, folder => new FolderViewModel(folder, _eventAggregator, _userProfileService, _userProfileFolderService));

        if (FolderViewModels != null)
        {
            FolderViewModels.Ascending = ascending;
        }

        RaisePropertyChanged(nameof(FolderViewModels));
    }

    private void OnUpdateFavoriteFolders()
    {
        LoadUserProfileFolderViewModels();
    }

    public DelegateCommand SyncChangesCommand { get; }
    private void SyncChanges()
    {
        _eventAggregator
            .GetEvent<SyncChangesEvent>()
            .Publish();
    }

    private void SyncBtnVisibilityChange()
    {
        RaisePropertyChanged(nameof(IsSyncChangesBtnVisible));
    }

    private bool HasAssistants()
    {
        return _applicationUser.IsAuthenticated && _userAssistantService.Get().Count > 0;
    }
    public bool IsSyncChangesBtnVisible => _applicationUser.IsAssistant || HasAssistants();
}
