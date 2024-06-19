using Avalonia;
using Chameleon.Application.Events;
using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfilesViewModel
    : SubPageViewModelBase
    , IUserProfilesViewModel
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserProfileFolderService _userProfileFolderService;
    private readonly IApplicationUser _currentUser;
    private readonly ISystemBrowserManager _systemBrowserManager;
    private readonly IAutomationService _automationService;
    private readonly IToastNotificationService _toastNotificationService;
    private readonly AppSettingsAutomation _settings;
    private readonly IAutomationBrowserService _automationBrowserService;

    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel> _scriptMapping;

    private const string TitlePage = "Profiles";

    public UserProfilesViewModel(
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService,
        ISystemBrowserManager systemBrowserManager,
        IApplicationUser currentUser,
        IAutomationService automationService,
        IToastNotificationService toastNotificationService,
        IAutomationBrowserService automationBrowserService
        )
    {
        _systemBrowserManager = systemBrowserManager;
        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;
        _currentUser = currentUser;
        _automationService = automationService;
        _toastNotificationService = toastNotificationService;
        _automationBrowserService = automationBrowserService;

        EventAggregator.GetEvent<DeleteUserProfileEvent>()
           .Subscribe(OnDeleteUserProfileEvent);

        EventAggregator.GetEvent<AfterCreateOrRemoveFolderEvent>()
            .Subscribe(OnHandleUserEvent);

        EventAggregator.GetEvent<SelectedChangeUserProfileEvent>()
            .Subscribe(OnSelectedChanged);

        EventAggregator.GetEvent<SavedUserProfileFolderEvent>()
            .Subscribe(OnSaveFolder);

        EventAggregator.GetEvent<UpdateStaleDataEvent>()
           .Subscribe(LoadAsync);

        _settings = new AppSettingsAutomation();
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);

        if (!Loaded)
        {
            IsWaiting = true;
            LoadAsync();

            IsWaiting = false;

            InitializeScripts();
        }

        OnHandleUserEvent();
    }

    private void InitializeScripts()
    {
        var scripts = _automationService.GetAll();

        _scriptMapping = new ObservableCollection<IAutomationScriptDescription,
            IAutomationScriptViewModel>(scripts, script => new AutomationScriptViewModel(script, _automationService));

        OnPropertyChanged(nameof(ScriptViewModels));
        OnPropertyChanged(nameof(SelectedBrowserItem));
    }

    private ObservableCollection<SystemBrovserItemViewModel> _browserItems;
    public ObservableCollection<SystemBrovserItemViewModel> BrowserItems
    {
        get
        {
            if (_browserItems == null)
            {
                _browserItems = new ObservableCollection<SystemBrovserItemViewModel>
                {
                    new SystemBrovserItemViewModel(SystemBrowserType.Brave),
                    new SystemBrovserItemViewModel(SystemBrowserType.Chrome)
                };
                SelectedBrowserItem = _browserItems[0];
            }

            return _browserItems;
        }
    }

    private SystemBrovserItemViewModel _selectedBrowserItem;
    public SystemBrovserItemViewModel SelectedBrowserItem
    {
        get
        {
            if (_selectedBrowserItem == null)
            {
                var lastSelectedBrowserString = _settings.LastSelectedBrowser;
                if (string.IsNullOrEmpty(lastSelectedBrowserString))
                {
                    return null;
                }

                if (Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum))
                {
                    _selectedBrowserItem = BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);
                }
                else
                {
                    _selectedBrowserItem = BrowserItems[0];
                }
            }
            return _selectedBrowserItem;
        }
        set
        {
            SetProperty(ref _selectedBrowserItem, value);
            _settings.LastSelectedBrowser = value.SystemBrowserType.ToString();
        }
    }

    private ObservableCollectionView<IAutomationScriptViewModel> _scriptViewModels;
    public ObservableCollectionView<IAutomationScriptViewModel> ScriptViewModels
    {
        get
        {
            if (_scriptViewModels == null && _mapping != null)
            {
                _scriptViewModels = new ObservableCollectionView<IAutomationScriptViewModel>(_scriptMapping);

                SelectedAutomationScript = ScriptViewModels.FirstOrDefault(s => s.Id == _settings.LastRunScriptId);
            }

            return _scriptViewModels;
        }
    }

    private bool _isVisibleRunButton = true;
    public bool IsVisibleRunButton
    {
        get => _isVisibleRunButton;
        set => SetProperty(ref _isVisibleRunButton, value);
    }

    private bool _isVisibleStopButton;
    public bool IsVisibleStopButton
    {
        get => _isVisibleStopButton;
        set => SetProperty(ref _isVisibleStopButton, value);
    }

    private bool _isVisibleWaitButton;
    public bool IsVisibleWaitButton
    {
        get => _isVisibleWaitButton;
        set => SetProperty(ref _isVisibleWaitButton, value);
    }

    private IAutomationScriptViewModel _selectedAutomationScript;
    public IAutomationScriptViewModel SelectedAutomationScript
    {
        get { return _selectedAutomationScript; }
        set
        {
            if (_selectedAutomationScript != value)
            {
                SetProperty(ref _selectedAutomationScript, value);
                OnPropertyChanged(nameof(IsSelectedScript));
                RunAutomationCommand.NotifyCanExecuteChanged();
                _settings.LastRunScriptId = value.Id;
            }
        }
    }

    public bool IsSelectedScript => SelectedAutomationScript != null;

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
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(IsProfilesExist));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
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
        OnHandleUserEvent();
    }
    public void Open(IUserProfileFolder? folder)
    {
        OnOpenFolder(folder);
        //_viewModels.Refresh();
        //Folder = folder;
        //OnPropertyChanged(nameof(Folder));
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

        if (folderId == default)
        {
            HasFolder = false;
            Filter = null;

            return;
        }

        HasFolder = true;
        Filter = profile => profile.FolderId == folderId;
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
    private void OnSelectedChanged(SelectedUserProfileEventArgs arr = null)
    {
        _selectedProfiles = _mapping.Where(profile => profile.IsSelected);
        SelectedCount = _selectedProfiles.Count();

        RunAutomationCommand.NotifyCanExecuteChanged();
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
            await DeleteProfiles();
    }

    private void OnDeleteUserProfileEvent(UserProfileEventArgs obj)
    {
        var profile = _mapping.FirstOrDefault(profile => profile.UserProfile.Id == obj.UserProfile.Id);
        if (profile != null)
            profile.IsSelected = false;

        _mapping.Remove(profile);
        _viewModels = null;
        OnPropertyChanged(nameof(ViewModels));
        OnSelectedChanged();
        OnHandleUserEvent();
    }
    private async Task DeleteProfiles()
    {
        var profiles = _selectedProfiles.ToList();

        foreach (var profile in profiles)
        {

            await Task.Run(() => _userProfileService.Delete(profile.UserProfile));
            profile.IsSelected = false;
            _mapping.Remove(profile);

            //var userProfile = profile.UserProfile;
            //EventAggregator.GetEvent<RemoveWebBrowserViewEvent>()
            //    .Publish(new UserProfileEventArgs(userProfile));

            //EventAggregator
            //    .GetEvent<DeleteUserProfileEvent>()
            //    .Publish(new UserProfileEventArgs(userProfile));


        }
        _viewModels = null;
        OnViewModelChange(this, EventArgs.Empty);
        ChangeProfilesInFavoriteFolder();
        OnSelectedChanged();
        OnHandleUserEvent();
    }

    [RelayCommand]
    private void RemoveProfilesFromFolder()
    {
        if (_folder.Id == 0 ||
            _selectedProfiles == null ||
            !_selectedProfiles.Any())
        {
            return;
        }

        var ids = _selectedProfiles
            .Select(a => a.UserProfile.Id)
            .ToList();

        _userProfileService.MoveUserProfileToFolder(ids, null);
        Filter = p => p.FolderId == _folder.Id;
        OnHandleUserEvent();
        ChangeProfilesInFavoriteFolder();
    }

    [RelayCommand]
    private async Task AddProfilesToFolder()
    {
        if (_folder.Id == 0)
            return;

        //_userProfilesPopupService.ShowPopup(_folder);

        if (await ContentDialogService.ShowAsync<IAddUserProfilesPopupView, IAddUserProfilesPopupViewModel>(
            viewModel =>
            {
                viewModel.Title = "Add Profiles";
                viewModel.Folder = _folder;
            }) == IContentDialogResult.Primary)
        {
            Filter = p => p.FolderId == _folder.Id;
            OnHandleUserEvent();
        }
    }

    [RelayCommand]
    private async Task MoveProfilesToFolder()
    {
        var selectedUserProfiles = _selectedProfiles
            .Select(p => (IUserProfile)p.UserProfile)
            .ToList();

        if (await ContentDialogService.ShowAsync<IMoveUserProfilesPopupView, IMoveUserProfilesPopupViewModel>(
             viewModel =>
             {
                 viewModel.Title = "Add To Folder";
                 viewModel.Profiles = selectedUserProfiles;
             }) == IContentDialogResult.Primary)
        {
            if (_folder.Id != 0)
            {
                Filter = p => p.FolderId == _folder.Id;
                OnHandleUserEvent();
            }
        }
    }

    private void ChangeProfilesInFavoriteFolder()
    {
        var folderId = Folder.Id;

        EventAggregator
            .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
            .Publish(new ChangeProfilesInFavoriteFolderEventArgs(folderId));
    }


    public async Task<IUserProfile> CreateNewProfile()
    {
        var folderId = HasFolder ? (int?)Folder.Id : null;

        var profile = await _userProfileService.CreateAsync(folderId);
        if (folderId != 0 && !ViewModels.Any(p => p.UserProfile.Id == profile.Id)) ;
        OnHandleUserEvent();

        return profile;
        //EventAggregator
        //    .GetEvent<CreateUserProfileEvent>()
        //    .Publish(new CreateUserProfileEventArgs(folderId));

        //EventAggregator.PublishPubSubEvent(new CreateUserProfileEventArgs(folderId));
        //OnHandleUserEvent();
    }
    //private void OnCreateUserProfileEvent(ChangeProfilesInFavoriteFolderEventArgs e)
    //{
    //    if (e.FolderId != 0 && !ViewModels.Any(p => p.UserProfile.Id == e.Profile.Id))
    //        OnHandleUserEvent();
    //        //_viewModels = null;

    //    //OnHandleUserEvent();

    //    if (e.Navigate == true)
    //        NavigationService.NavigateToType(typeof(IUserProfileIdentityView), e.Profile);

    //    //IsDisabledCreateNewProfile = false;
    //}

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

        profiles.ForEach(async profile =>
        {
            await profile.OpenUserBrowser();
        });

        //TODO: ? EventAggregator
        //    .GetEvent<OpenMainWindowByIndexEvent>()
        //    .Publish(new OpenMainWindowByIndexEventArgs(2));

    }

    [RelayCommand]
    private void RunAutomation()
    {
        if (!ViewModels.Any(p => p.IsSelected == true))
        {
            _toastNotificationService.ShowInformation("Select one or more profiles to run the automation.");
            return;
        }
        if (SelectedAutomationScript == null)
        {
            _toastNotificationService.ShowInformation("Select an automation.");
            return;
        }

        Task.Run(RunAutomationAsync);
        IsVisibleRunButton = false;
        IsVisibleStopButton = true;
    }

    private async Task RunAutomationAsync()
    {
        var script = new AutomationScriptDescription
        {
            Id = SelectedAutomationScript.Id,
            Title = SelectedAutomationScript.Title,
            Description = SelectedAutomationScript.Description,
            Parameters = SelectedAutomationScript.Parameters
            .Select(sp => (IAutomationParameterValue)new AutomationParameterValue
            {
                Name = sp.Name,
                Value = sp.Value,
                ParameterId = sp.Id
            }).ToList()
        };
        var profiles = _selectedProfiles.Select(p => (IUserProfile)p.UserProfile).ToList();
        await _automationBrowserService.RunScript(script, SelectedBrowserItem.SystemBrowserType, profiles);
    }

    [RelayCommand]
    private void StopAutomation()
    {
        Task.Run(StopAutomationAsync);
        IsVisibleStopButton = false;
        IsVisibleWaitButton = true;
    }

    private async Task StopAutomationAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(10));

        this.DispatcherService.InvokeOnUiThread(() =>
        {
            IsVisibleWaitButton = false;
            IsVisibleRunButton = true;
        });
    }

    private void OpenSystemBrowser(SystemBrowserType browserType)
    {
        var profiles = GetSelectedProfiles();

        profiles.ForEach(async (selectedProfile) =>
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
            (_userProfileService, profile as UserProfile, _currentUser, _systemBrowserManager));

        _mapping.CollectionChanged += OnViewModelChange;

        OnHandleUserEvent();
    }

    private void ApplySearchFilter()
    {
        var searchText = SearchText?.ToLower();
        var hasSearchText = !string.IsNullOrWhiteSpace(SearchText);
        var isInCurrentFolder = Folder?.CreatorUserId != null;

        Filter = profile => FilterByFolder(profile, hasSearchText, isInCurrentFolder, searchText);

        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(IsProfilesExist));
        OnPropertyChanged(nameof(HasNoItems));
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

    public async void OnFilterTo(IUserProfile p = null)
    {
        while (!Loaded)
            await Task.Delay(250);

        if (p != null)
        {
            if (p.FolderId is int fid && fid != 0)
                ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().SetSelectedById(fid);
            else
                //ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
                //await ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null); 

                Filter = profile => p.Id == profile.Id;
        }
        else
        {
            //Filter = profile => 0 == profile.Id;
            ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().SetSelectedById(0);
            Filter = null;
            //ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
        }

        OnHandleUserEvent();
    }
    private bool _isWaiting = true;
    public bool IsWaiting
    {
        get => _isWaiting;
        set => SetProperty(ref _isWaiting, value);
    }

    public bool IsProfilesExist => _mapping?.Any() == true;

    public bool HasNoItems =>
        !SearchText.HasAny() &&
        ViewModels == null ||
        ViewModels.Count == 0;
}