using Avalonia.Controls;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Services;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;



public partial class UserProfilesViewModel
    : SubPageViewModelBase
    , IUserProfilesViewModel
{
    private readonly IUserProfileService _userProfileService;
    //TODO: private readonly IUserProfilesPopupService _userProfilesPopupService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IApplicationUser _currentUser;

    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;

    private const string TitlePage = "Profiles";

    public UserProfilesViewModel(
        IUserProfileService userProfileService,
        //IUserProfilesPopupService userProfilesPopupService,
        IUserProfileFolderService userProfileFolderService,
        IApplicationUser currentUser)
    {
        _userProfileService = userProfileService;
        //_userProfilesPopupService = userProfilesPopupService;
        _userProfileFolderService = userProfileFolderService;
        _currentUser = currentUser;

        //EventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .SubscribeOnce(OnAuthenticated);

        EventAggregator
           .GetEvent<DeleteUserProfileEvent>()
           .Subscribe(OnDeleteUserProfileEvent);

        EventAggregator
           .GetEvent<DeleteUserProfileFolderEvent>()
           .Subscribe(OnHandleUserEvent);

        EventAggregator
           .GetEvent<CreateUserProfileEvent>()
           .Subscribe(OnHandleUserEvent);

        EventAggregator
            .GetEvent<OpenUserProfileFolderEvent>()
            .Subscribe(args => OnOpenFolder(args.UserProfileFolder));

        EventAggregator
            .GetEvent<CreateNewUserProfileEvent>()
            .Subscribe(CreateNewProfile);

        EventAggregator
            .GetEvent<SelectedChangeUserProfileEvent>()
            .Subscribe(_ => OnSelectedChanged());

        EventAggregator
            .GetEvent<AddUserProfileToFolderEvent>()
            .Subscribe(OnHandleUserEvent);

        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Subscribe(args => UpdateProfilesInFolder());

        EventAggregator
            .GetEvent<SavedUserProfileFolderEvent>()
            .Subscribe(OnSaveFolder);

        EventAggregator
           .GetEvent<UpdateStaleDataEvent>()
           .Subscribe(LoadAsync);
    }
    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            OnAuthenticated();
        }

        OnHandleUserEvent();
    }

    private void UpdateProfilesInFolder()
    {
        DispatcherService.InvokeOnUiThread(() =>
        {
            OnViewModelChange(this, EventArgs.Empty);
            SetViewModelsFilter();
            OnPropertyChanged(nameof(ViewModels));
            OnPropertyChanged(nameof(HasNoItems));
            OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
        });
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                //TODO:?_debounceTimer.Debounce(2000, ApplySearchFilter);
                ApplySearchFilter();
            }
        }
    }

    public bool ShowFavoriteIcon => Folder?.Id > 0;

    private void OnHandleUserEvent()
    {
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
    }

    private void OnDeleteUserProfileEvent(UserProfileEventArgs obj)
    {
        var profile = _mapping.FirstOrDefault(profile => profile.UserProfile.Id == obj.UserProfile.Id);
        if (profile != null)
            profile.IsSelected = false;

        OnSelectedChanged();
        OnHandleUserEvent();
    }

    private void OnHandleUserEvent(object obj)
    {
        OnHandleUserEvent();
    }

    public bool HasProfileWithoutFolder => _mapping != null && _mapping.Any(profile => !profile.UserProfile.FolderId.HasValue);
    public IApplicationUser CurrentUser => _currentUser;
    public bool IsAddProfilesToFolderCommandEnabled => HasProfileWithoutFolder && !CurrentUser.IsAssistant && Folder?.Id != 0;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(Folder);

    private PaginatorViewModel _paginatorViewModel;
    public PaginatorViewModel PaginatorViewModel
    {
        get => _paginatorViewModel;
        set
        {
            if (SetProperty(ref _paginatorViewModel, value))
            {
                _paginatorViewModel.ChangePageIndex += OnChangePage;
            }
        }
    }

    private void OnSaveFolder(UserProfileFolderEventArgs args)
    {
        OnPropertyChanged(nameof(Folder));

        if (HasFolder)
        {
            //var lastCrumbs = BreadcrumbsViewModel.Breadcrumbs.Last();
            //lastCrumbs.Title = Folder.Title;
        }
    }

    private void OnChangePage(object sender, EventArgs args)
    {
        ViewModels.Offset = PaginatorViewModel.Skip;
    }

    private void OnViewModelChange(object sender, EventArgs args)
    {
        var items = ViewModels.Filter == null ? _mapping.ToList() : _mapping.Where(ViewModels.Filter).ToList();
        int count = items.Count;

        PaginatorViewModel.TotalCount = count;
        TotalCount = count;
    }

    private void OnOpenFolder(IUserProfileFolder userProfileFolder)
    {
        Folder = userProfileFolder;

        UnselectItems();
        OnPropertyChanged(nameof(HasNoItems));
    }

    private IUserProfileFolder? _folder;
    public IUserProfileFolder? Folder
    {
        get
        {
            return _folder;
        }
        set
        {
            if (SetProperty(ref _folder, value))
            {
                UpdateFolder();
                UpdateBreadcrumbsViewModel();
            }

            OnPropertyChanged(nameof(ShowFavoriteIcon));
            OnPropertyChanged(nameof(IsSharedFolder));
            OnPropertyChanged(nameof(SelectedFolderTitle));
        }
    }
    public string SelectedFolderTitle => Folder?.Title ?? "All profiles";

    private void UpdateBreadcrumbsViewModel()
    {
        //var root = BreadcrumbsViewModel.Root;

        //if (root == null)
        //{
        //    return;
        //}

        //root.HasContinuation = HasFolder;
        //root.IsBold = HasFolder;

        //var breadcrumbs = BreadcrumbsViewModel.Breadcrumbs;

        //if (breadcrumbs.Count > 1)
        //{
        //    breadcrumbs.Remove(breadcrumbs[1]);
        //}

        //if (HasFolder)
        //{
        //    var folderBreadcrumb = new BreadcrumbViewModel()
        //    {
        //        Title = Folder.Title
        //    };

        //    breadcrumbs.Add(folderBreadcrumb);
        //}
    }
    private void UpdateFolder()
    {
        SearchText = string.Empty;

        int folderId = Folder.Id;

        if (folderId == default(int))
        {
            HasFolder = false;
            Filter = null;

            return;
        }

        HasFolder = true;
        Filter = profile => profile.FolderId == folderId;
    }

    public bool HasNoItems
    {
        get
        {
            if (ViewModels == null)
            {
                return true;
            }

            return ViewModels.Count == 0;
        }
    }

    private int _totalCount;
    public int TotalCount
    {
        get => _totalCount;
        set
        {
            SetProperty(ref _totalCount, value);
        }
    }

    private int _selectedCount;
    public int SelectedCount
    {
        get => _selectedCount;
        set
        {
            if (SetProperty(ref _selectedCount, value))
            {
                OnPropertyChanged(nameof(HasSelectedItems));
            }
        }
    }

    public bool HasSelectedItems => ViewModels != null && ViewModels.Count(v => v.IsSelected) > 0;

    private IEnumerable<UserProfileViewModel> _selectedProfiles;
    private void OnSelectedChanged()
    {
        _selectedProfiles = _mapping.Where(profile => profile.IsSelected);
        SelectedCount = _selectedProfiles.Count();
    }

    private bool _hasFolder;
    public bool HasFolder
    {
        get => _hasFolder;
        set
        {
            SetProperty(ref _hasFolder, value);
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        if (_mapping == null)
        {
            return;
        }

        foreach (var profile in ViewModels)
        {
            profile.IsSelected = true;
        }

        SelectedCount = ViewModels.Count;
    }

    [RelayCommand]
    private void SelectAllProfilesFromFolder()
    {
        if (_mapping == null)
        {
            return;
        }

        var profiles = _mapping
            .Where(p => p.UserProfile.FolderId == Folder.Id || Folder.Id == 0)
            .ToList();

        profiles.ForEach(p => p.IsSelected = true);
        SelectedCount = profiles.Count;
    }

    [RelayCommand]
    private void UnselectItems()
    {
        if (_selectedProfiles == null)
        {
            return;
        }

        foreach (var profile in _selectedProfiles)
        {
            profile.IsSelected = false;
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (await MesageBoxHelper.ShowAsync("Delete User Profiles", 
            $"Are you sure you want to delete {SelectedCount} profiles?",
            ContentDialogButtons.YesNo, 
            "DeleteLines"))
            DispatcherService.InvokeOnUiThread(DeleteProfiles);
    }

    private void DeleteProfiles()
    {
        var profiles = _selectedProfiles.ToList();

        foreach (var profile in profiles)
        {
            var userProfile = profile.UserProfile;

            EventAggregator.GetEvent<RemoveWebBrowserViewEvent>()
                .Publish(new UserProfileEventArgs(userProfile));

            EventAggregator
                .GetEvent<DeleteUserProfileEvent>()
                .Publish(new UserProfileEventArgs(userProfile));

            OnViewModelChange(this, EventArgs.Empty);
            ChangeProfilesInFavoriteFolder();
        }

        OnSelectedChanged();
    }

    [RelayCommand]
    private void RemoveProfilesFromFolder()
    {
        if (_selectedProfiles== null || !_selectedProfiles.Any())
        {
            return;
        }

        var ids = _selectedProfiles
            .Select(a => a.UserProfile.Id)
            .ToList();

        _userProfileService.MoveUserProfileToFolder(ids, null);
        foreach (var profile in _selectedProfiles)
        {
            profile.IsSelected = false;
        }

        ChangeProfilesInFavoriteFolder();
        OnHandleUserEvent();
        UpdateProfilesInFolder();
    }

    [RelayCommand]
    private async Task AddProfilesToFolder()
    {
        if (_folder.Id == 0)
            return;

        //_userProfilesPopupService.ShowPopup(_folder);

       await ContentDialogService.ShowAsync<IAddUserProfilesPopupView, IAddUserProfilesPopupViewModel>(
           viewModel =>
           {
               viewModel.Title = "Add Profiles";
               viewModel.Folder = _folder;
           });

        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));

        ChangeProfilesInFavoriteFolder();
    }

    [RelayCommand]
    private void MoveProfilesToFolder()
    {
        var selectedUserProfiles = _selectedProfiles
            .Select(p => p.UserProfile)
            .ToList();

        ContentDialogService.ShowAsync<IMoveUserProfilesPopupView, IMoveUserProfilesPopupViewModel>(
            viewModel =>
            {
                viewModel.Title = "Add To Folder";
                viewModel.Profiles = selectedUserProfiles;
            });
    }

    private void ChangeProfilesInFavoriteFolder()
    {
        var folderId = Folder.Id;

        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Publish(new ChangeProfilesInFavoriteFolderEventArgs(folderId));
    }

    public static bool IsDisabledCreateNewProfile = false;
    private void CreateNewProfile()
    {
        if (IsDisabledCreateNewProfile)
        {
            return;
        }
        //TODO: Remove hardcode
        IsDisabledCreateNewProfile = true;

        var folderId = HasFolder ? (int?)Folder.Id : null;

        EventAggregator
            .GetEvent<CreateUserProfileEvent>()
            .Publish(new CreateUserProfileEventArgs(folderId));

        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
    }

    private ObservableCollectionView<UserProfileViewModel> _viewModels;
    public ObservableCollectionView<UserProfileViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<UserProfileViewModel>(_mapping)
                {
                    TrackItemChanges = true,
                    TrackCollectionChanges = true,
                    Order = profile => profile.Title
                };

                InitPaginator();
                SetViewModelsFilter();
                OnPropertyChanged(nameof(HasNoItems));
                OnPropertyChanged(nameof(HasSelectedItems));
            }

            return _viewModels;
        }
    }
    private void InitPaginator()
    {
        PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
        ViewModels.Offset = PaginatorViewModel.Skip;
        ViewModels.Limit = PaginatorViewModel.OnPageItems;
        TotalCount = PaginatorViewModel.TotalCount;
    }

    [RelayCommand]
    private void OpenFirefox()
    {
        OpenSystemBrowser(SystemBrowserType.Firefox);
    }

    [RelayCommand]
    private void OpenChrome()
    {
        OpenSystemBrowser(SystemBrowserType.Chrome);
    }

    [RelayCommand]
    private void OpenBrave()
    {
        OpenSystemBrowser(SystemBrowserType.Brave);
    }

    [RelayCommand]
    private void OpenChameleonBrowser()
    {
        var profiles = GetSelectedProfiles();

        profiles.ForEach(profile =>
        {
            EventAggregator
                .GetEvent<OpenUserBrowserEvent>()
                .Publish(new UserProfileEventArgs(profile.UserProfile));
        });

        //TODO: ? EventAggregator
        //    .GetEvent<OpenMainWindowByIndexEvent>()
        //    .Publish(new OpenMainWindowByIndexEventArgs(2));

    }

    private void OpenSystemBrowser(SystemBrowserType browserType)
    {
        var profiles = GetSelectedProfiles();

        profiles.ForEach(async(selectedProfile) =>
        {
            await selectedProfile.OpenSystemBrowser(browserType);
            //var profile = selectedProfile.UserProfile;
            //var args = new UserProfileSystemBrowserEventArgs(profile, browserType);

            //EventAggregator
            //    .GetEvent<OpenUserSystemBrowserEvent>()
            //    .Publish(args);
        });
    }

    private List<UserProfileViewModel> GetSelectedProfiles()
    {
        return _selectedProfiles.ToList();
    }

    private Func<IUserProfile, bool> _filter;
    public Func<IUserProfile, bool> Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
            {
                SetViewModelsFilter();
            }
        }
    }

    private void SetViewModelsFilter()
    {
        if (_viewModels == null)
        {
            return;
        }

        if (_filter == null)
        {
            _viewModels.Filter = null;
        }
        else
        {
            _viewModels.Filter = (viewModel) => _filter(viewModel.UserProfile);
        }

        OnViewModelChange(this, EventArgs.Empty);
    }

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
                PaginatorViewModel.PageIndex = 0;
            }
        }
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

        //DispatcherService.InvokeOnUiThreadAsync(
        //    () => LoadAsync(),
        //    _ => IsWaiting = false
        //    );

        LoadAsync();

        IsWaiting = false;
    }

    private void LoadAsync()
    {
        ViewModels?.Clear();
        _viewModels = null;

        var userProfiles = _userProfileService.GetAll();

        _mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(
        userProfiles, profile => new UserProfileViewModel
            (_userProfileService, profile, _currentUser));

        _mapping.CollectionChanged += OnViewModelChange;

        //ApplySearchFilter();

        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(IsProfilesExist));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
    }

    private void ApplySearchFilter()
    {
        var searchText = SearchText?.ToLower();
        var hasSearchText = !string.IsNullOrWhiteSpace(SearchText);
        var isInCurrentFolder = Folder?.CreatorUserId != null;

        Filter = profile => FilterByFolder(profile, hasSearchText, isInCurrentFolder, searchText);

        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(IsProfilesExist));
    }

    private bool FilterByFolder(IUserProfile profile, bool hasSearchText, bool isInCurrentFolder, string searchText)
    {
        if (!hasSearchText && isInCurrentFolder)
        {
            return profile.FolderId == Folder.Id;
        }
        if (hasSearchText && isInCurrentFolder)
        {
            return profile.FolderId == Folder.Id && FilterByUserProfile(profile, searchText);
        }
        if (hasSearchText)
        {
            return FilterByUserProfile(profile, searchText);
        }

        return true;
    }

    private bool FilterByUserProfile(IUserProfile profile, string searchText)
    {
        return profile.Title.ToLower().Contains(searchText);
    }

    public void Refresh()
    {
        _viewModels.Refresh();
        Folder = new UserProfileFolder { Title = "All profiles" };
        OnPropertyChanged(nameof(Folder));
    }

    public bool IsProfilesExist => ViewModels != null &&
                                   ViewModels.Count > 0 ||
                                   Folder?.ProfilesCount == 0 &&
                                   Folder?.Id != 0;

    public async void OnFilterTo(IUserProfile p = null)
    {
        while (!Loaded)
            await Task.Delay(250);

        if (p != null)
        {
            Filter = profile => p.Id == profile.Id;
            if (p.FolderId is int fid)
                ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().SetSelectedById(fid);
            else
                ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
        }
        else
        {
            Filter = null;
            ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
        }

        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(IsProfilesExist));
    }
}