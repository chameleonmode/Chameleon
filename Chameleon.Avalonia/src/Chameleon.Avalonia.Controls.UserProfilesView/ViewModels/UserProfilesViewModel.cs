using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Infrastructure.Settings;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.Events;
using Chameleon.Interfaces.App.Automation.Services;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;
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
    private readonly IAutomationBrowserService _automationBrowserService;

    private readonly ObservableCollection<SystemBrovserItemViewModel> _browserItems =
    [
        new SystemBrovserItemViewModel(SystemBrowserType.Brave),
        new SystemBrovserItemViewModel(SystemBrowserType.Chrome)
    ];

    private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
    private ObservableCollection<IAutomationScriptDescription, IAutomationScriptViewModel> _scriptMapping;

    private IEnumerable<UserProfileViewModel> _selectedProfiles;

    private SystemBrovserItemViewModel _selectedBrowserItem;
    private CancellationTokenSource _cts;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private bool _isWaiting = true;

    [ObservableProperty]
    private bool _hasFolder;

    [ObservableProperty]
    private bool _isVisibleRunButton = true;

    [ObservableProperty]
    private bool _isVisibleStopButton;

    [ObservableProperty]
    private bool _isVisibleWaitButton;

    [ObservableProperty]
    private bool _isRecordSelected;

    private List<IUserProfileActionsViewModel> GetSelectedProfiles => _selectedProfiles.Cast<IUserProfileActionsViewModel>().ToList();
    public bool HasSelectedItems => ViewModels != null && ViewModels.Any(v => v.IsSelected);
    public bool IsProfilesExist => _mapping?.Any() == true;
    public bool HasNoItems => !SearchText.HasAny() && ViewModels == null || ViewModels.Count == 0;
    public bool IsSelectedScript => SelectedAutomationScript != null;
    public bool ShowFavoriteIcon => Folder?.Id > 0;
    public bool HasProfileWithoutFolder => _mapping != null && _mapping.Any(profile => !profile.UserProfile.FolderId.HasValue);
    public IApplicationUser CurrentUser => _currentUser;
    public bool IsAddProfilesToFolderCommandEnabled => HasProfileWithoutFolder && !CurrentUser.IsAssistant && Folder?.Id != 0;
    public bool IsSharedFolder => _userProfileFolderService.IsSharedFolder(Folder);
    public string SelectedFolderTitle => Folder?.Title ?? "All profiles";
    public ObservableCollection<SystemBrovserItemViewModel> BrowserItems => _browserItems;

    private CancellationToken RecreateCancellationToken
    {
        get
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();
            return _cts.Token;
        }
    }

    public SystemBrovserItemViewModel SelectedBrowserItem
    {
        get => _selectedBrowserItem;
        set
        {
            SetProperty(ref _selectedBrowserItem, value);
            ConfigHelper.LastSelectedBrowser = value.SystemBrowserType.ToString();
        }
    }

    private ObservableCollectionView<IAutomationScriptViewModel> _scriptViewModels;
    public ObservableCollectionView<IAutomationScriptViewModel> ScriptViewModels
    {
        get
        {
            if (_scriptViewModels == null && _scriptMapping != null)
            {
                _scriptViewModels = new ObservableCollectionView<IAutomationScriptViewModel>(_scriptMapping);
            }

            return _scriptViewModels;
        }
    }


    private IAutomationScriptViewModel _selectedAutomationScript;
    public IAutomationScriptViewModel SelectedAutomationScript
    {
        get { return _selectedAutomationScript; }
        set
        {
            if (value != null && _selectedAutomationScript != value)
            {
                SetProperty(ref _selectedAutomationScript, value);
                OnPropertyChanged(nameof(IsSelectedScript));
                RunAutomationCommand.NotifyCanExecuteChanged();
                ConfigHelper.LastRunScriptId = value.Id;
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
                ApplySearchFilter();
            }
        }
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
                //UpdateFolder();
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

            OnPropertyChanged(nameof(ShowFavoriteIcon));
            OnPropertyChanged(nameof(IsSharedFolder));
            OnPropertyChanged(nameof(SelectedFolderTitle));
        }
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

                //InitPaginator();
                PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
                ViewModels.Offset = PaginatorViewModel.Skip;
                ViewModels.Limit = PaginatorViewModel.OnPageItems;
                TotalCount = PaginatorViewModel.TotalCount;
                SetViewModelsFilter();
                OnPropertyChanged(nameof(HasNoItems));
                OnPropertyChanged(nameof(HasSelectedItems));
            }

            return _viewModels;
        }
    }

    private PaginatorViewModel _paginatorViewModel;
    public PaginatorViewModel PaginatorViewModel
    {
        get => _paginatorViewModel;
        set
        {
            if (SetProperty(ref _paginatorViewModel, value))
            {
                _paginatorViewModel.ChangePageIndex += (s, a) => { ViewModels.Offset = PaginatorViewModel.Skip; };
            }
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

    public UserProfilesViewModel(
        IUserProfileService userProfileService,
        IUserProfileFolderService userProfileFolderService,
        ISystemBrowserManager systemBrowserManager,
        IApplicationUser currentUser,
        IAutomationService automationService,
        IAutomationBrowserService automationBrowserService
        )
    {
        _systemBrowserManager = systemBrowserManager;
        _userProfileService = userProfileService;
        _userProfileFolderService = userProfileFolderService;
        _currentUser = currentUser;
        _automationService = automationService;
        _automationBrowserService = automationBrowserService;

        EventAggregator.Sub<DeleteUserProfileEvent, UserProfileEventArgs>(OnDeleteUserProfileEvent);

        EventAggregator.Sub<AfterCreateOrRemoveFolderEvent>(OnHandleUserEvent);

        EventAggregator.GetEvent<SelectedChangeUserProfileEvent>()
            .Subscribe(OnSelectedChanged);

        EventAggregator.GetEvent<SavedUserProfileFolderEvent>()
            .Subscribe((e) => OnPropertyChanged(nameof(Folder)));

        EventAggregator.GetEvent<UpdateStaleDataEvent>()
           .Subscribe(LoadAsync);
    }

    public override async Task InitAsync(object? param)
    {      
        IsWaiting = true;

        await base.InitAsync(param);

        if (!Loaded)
        {
            LoadAsync();
        }

        await InitializeScripts();
        InintializeLastSelectedAutomation();

        OnHandleUserEvent();  

        IsWaiting = false;
    }

    private async Task InitializeScripts()
    {
        var scripts = await _automationService.GetAll();
        var usd = ConfigHelper.UserScriptsDirectory;
        if (!usd.HasAny())
        {
            var appSetting = await ApplicationSettingsService.Instance.GetAsync();
            usd = appSetting.Settings.UserScriptsDirectory;
        }
        scripts.AddRange(await _automationService.GetAll(usd));

        _scriptViewModels = null;
        _scriptMapping = new ObservableCollection<IAutomationScriptDescription,
            IAutomationScriptViewModel>(scripts, script => new AutomationScriptViewModel(script));

        OnPropertyChanged(nameof(ScriptViewModels));
        OnPropertyChanged(nameof(SelectedBrowserItem));
    }

    private void InintializeLastSelectedAutomation()
    {
        var lastSelectedBrowserString = ConfigHelper.LastSelectedBrowser;

        if (!lastSelectedBrowserString.HasAny() ||
            !Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum))
        {
            SelectedBrowserItem = BrowserItems[0];
        }
        else
        {
            SelectedBrowserItem = BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);
        }

        SelectedAutomationScript = ScriptViewModels.FirstOrDefault(s => s.Id == ConfigHelper.LastRunScriptId);
        if (SelectedAutomationScript is null && ScriptViewModels.Count > 0)
            SelectedAutomationScript = ScriptViewModels[0];
    }

    private void OnHandleUserEvent()
    {
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasNoItems));
        OnPropertyChanged(nameof(IsProfilesExist));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(HasProfileWithoutFolder));
        OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
    }

    private void OnViewModelChange(object sender, EventArgs args)
    {
        var items = ViewModels.Filter == null ? _mapping.ToList() : _mapping.Where(ViewModels.Filter).ToList();
        int count = items.Count;

        PaginatorViewModel.TotalCount = count;
        TotalCount = count;
    }

    public void Open(IUserProfileFolder? folder)
    {
        Folder = folder;

        UnselectItems();
        OnHandleUserEvent();
    }


    private void OnSelectedChanged(SelectedUserProfileEventArgs arr = null)
    {
        _selectedProfiles = _mapping.Where(profile => profile.IsSelected);
        SelectedCount = _selectedProfiles.Count();

        RunAutomationCommand.NotifyCanExecuteChanged();
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
        {
            var profiles = _selectedProfiles.ToList();

            foreach (var profile in profiles)
            {
                await Task.Run(() => _userProfileService.Delete(profile.UserProfile));
                profile.IsSelected = false;
                _mapping.Remove(profile);
            }
            _viewModels = null;
            OnViewModelChange(this, EventArgs.Empty);
            ChangeProfilesInFavoriteFolder();
            OnSelectedChanged();
            OnHandleUserEvent();
        }
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
        if (ViewModels.Count == 1 || folderId != 0 && !ViewModels.Any(p => p.UserProfile.Id == profile.Id))
            OnHandleUserEvent();

        return profile;
    }


    [RelayCommand]
    private void OpenChameleonBrowser()
    {
        GetSelectedProfiles.ForEach(profile =>
        {
            profile.OpenUserBrowser();
        });
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

    private void OpenSystemBrowser(SystemBrowserType browserType)
    {
        GetSelectedProfiles.ForEach(async (selectedProfile) =>
        {
            await selectedProfile.OpenSystemBrowser(browserType);
        });
    }

    [RelayCommand]
    private async Task RunAutomation()
    {
        if (!ViewModels.Any(p => p.IsSelected == true))
        {
            //_toastNotificationService.ShowInformation("Select one or more profiles to run the automation.");
            await MesageBoxHelper.ShowErrorAsync(
                "Select",
                "Select one or more profiles to run the automation.");
            return;
        }
        if (SelectedAutomationScript == null)
        {
            //_toastNotificationService.ShowInformation("Select an automation.");
            await MesageBoxHelper.ShowErrorAsync(
                "Select",
                "Select an automation.");
            return;
        }

        IsVisibleRunButton = false;
        IsVisibleStopButton = true;

        //await RunAutomationAsync();
        var script = new AutomationScriptDescription
        {
            Id = SelectedAutomationScript.Id,
            Title = SelectedAutomationScript.Title,
            Description = SelectedAutomationScript.Description,
            FilePath = SelectedAutomationScript.Filepath,
            Parameters = SelectedAutomationScript.Parameters.Select(
                sp => (IAutomationParameterValue)new AutomationParameterValue
                {
                    Name = sp.Name,
                    Value = sp.Value,
                    ParameterId = sp.Id
                }).ToList()
        };

        //var token = RecreateCancellationToken;
        await _automationBrowserService.RunScript(script, SelectedBrowserItem.SystemBrowserType, GetSelectedProfiles, RecreateCancellationToken, IsRecordSelected);

        IsVisibleRunButton = true;
        IsVisibleStopButton = false;
        IsVisibleWaitButton = false;
    }


    [RelayCommand]
    private void StopAutomation()
    {
        IsVisibleStopButton = false;
        IsVisibleWaitButton = true;
        _cts.Cancel();
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

    private static bool FilterByUserProfile(IUserProfile profile, string searchText)
    {
        return profile.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
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
                await ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);

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
}