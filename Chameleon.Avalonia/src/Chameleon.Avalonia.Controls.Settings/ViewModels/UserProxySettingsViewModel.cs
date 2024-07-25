using AutoMapper;
using Chameleon.App.Shared.Proxies;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.Proxies;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Core.Extensions;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.Dialogs;
using Chameleon.CT.Common.Collections;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using System.Threading;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class UserProxySettingsViewModel
       : SubPageViewModelBase
       , IUserProxySettingsViewModel
{
    private readonly IMapper _mapper;
    private readonly IProxyService _proxyService;
    private readonly IUserProfileService _userProfileService;
    private readonly IProxyAccessViewModels _proxyAccessViewModels;
    private ObservableCollection<IUserProfile, UserProxySettingViewModel> _mapping;
    private const int CountProxies = 5;

    private ObservableCollection<IUserProfileFolder, ProfileFolderViewModel> _folderMapping;
    private readonly IUserProfileFolderService _userProfileFolderService;

    public UserProxySettingsViewModel(
        IMapper mapper,
        IUserProfileService userProfileService,
        IProxyService proxyService,
        IProxyAccessViewModels proxyAccessViewModels,
        IUserProfileFolderService userProfileFolderService)
    {
        Title = "Proxy";

        _mapper = mapper;
        _proxyService = proxyService;
        _userProfileService = userProfileService;
        _proxyAccessViewModels = proxyAccessViewModels;
        _proxyAccessViewModels.AddItems(CountProxies);
        _userProfileFolderService = userProfileFolderService;

        //EventAggregator
        //    .GetEvent<SavedUserProfileEvent>()
        //    .Subscribe(args => OnUserProfileSaved());

        EventAggregator
          .GetEvent<UpdateStaleDataEvent>()
          .Subscribe(() => OnUpdateStaleDataEvent());

        //EventAggregator
        //    .GetEvent<ChangeProfilesInFavoriteFolderEvent>()
        //    .Subscribe(UpdateProfilesInFolder);

        EventAggregator
            .GetEvent<SelectedChangeUserProfileEvent>()
            .Subscribe(args => OnUserProfileSelected());

        EventAggregator
           .GetEvent<SelectedUserProxySettingEvent>()
           .Subscribe(args => OnSelectedChanged(args));

        EventAggregator
            .GetEvent<UserProxySetFolderIdEvent>()
            .Subscribe(args => FolderId = args.FolderId);

        EventAggregator
           .GetEvent<AfterCreateOrRemoveFolderEvent>()
           .Subscribe(OnAfterCreateOrRemoveFolderEvent);

        EventAggregator
            .GetEvent<RenameFolderEvent>()
            .Subscribe(args => OnRenameFolder(args.FolderId, args.Title));
    }

    private void OnAfterCreateOrRemoveFolderEvent()
    {
        _initFolderViewModels = true;
        OnPropertyChanged(nameof(FolderViewModels));
        if (FolderId != 0 && !FolderViewModels.Any(f => f.Id == FolderId))
        {
            FolderId = 0;   //TODO: combobox no update when folder delete of selected folder
        }
    }

    public override async Task InitAsync(object? param)
    {
        await base.InitAsync(param);
        if (!Loaded)
        {                                      
            await LoadUserProfileFolderViewModels();
            await InitializeViewModels();
            await InitializeCountriesAsync();
            SetFilter();
        }

        OnPropertyChanged(string.Empty);
    }
    public override async Task OnNavigatedToAsync(object? param)
    {
        await base.OnNavigatedToAsync(param);
        if (param is IUserProfileFolder folderId)
        {
            await LoadedTCS.Task;

            _folderId = 0;
            _selectedFolder = null;
            FolderId = folderId.Id;
        }
    }
    private void OnRenameFolder(int folderId, string title)
    {
        var item = FolderViewModels.First(a => a.Id == folderId);
        item.Title = title;
    }

    public ObservableCollection<IProxyCountry> Countries { get; private set; } = []; 
    public IProxyCountry Country
    {
        get => _proxyService.CurrentCountry;
        set
        {
            if (_proxyService.CurrentCountry != value)
            {
                _proxyService.CurrentCountry = value;
                OnPropertyChanged();
                UpdateProxyAccessAsync();
            }
        }
    }
    private async Task InitializeCountriesAsync()
    {
        Countries.Clear();

        foreach (var item in await Task.Run(() => _proxyService.GetCountries()))
            Countries.Add(item);

        Country = Countries.FirstOrDefault();
    }

    static readonly SemaphoreSlim initializeViewModelsSlim = new SemaphoreSlim(1, 1);
    private async Task InitializeViewModels()
    {
        await initializeViewModelsSlim.WaitAsync();
        
        ViewModels?.Clear();
        var userProfiles = await _userProfileService.GetAllAsync();

        _mapping = new ObservableCollection<IUserProfile, UserProxySettingViewModel>(
            userProfiles, userProfile => new UserProxySettingViewModel(_mapper, userProfile, EventAggregator)
            );

        _initViewModels = true;
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(FillProxiesIsEnabled));

        initializeViewModelsSlim.Release();
    }
    static readonly SemaphoreSlim initializeFolderViewModelsSlim = new SemaphoreSlim(1, 1);
    private async Task LoadUserProfileFolderViewModels()
    {
        await initializeFolderViewModelsSlim.WaitAsync();
        FolderViewModels?.Clear();

        var folders = await _userProfileFolderService.GetAllAsync();

        _folderMapping = new ObservableCollection<IUserProfileFolder, ProfileFolderViewModel>(
            folders, folder => new ProfileFolderViewModel(folder.Id, folder.Title));

        _initFolderViewModels = true;
        OnPropertyChanged(nameof(FolderViewModels));

        initializeFolderViewModelsSlim.Release();
    }

    private void UpdateProxyAccessAsync()
    {
        IsGettingAccess = true;
        DispatcherService.InvokeOnUiThreadAsync(UpdateProxyAccess,
            null, () => IsGettingAccess = false);
    }

    private bool _isGettingAccess;
    public bool IsGettingAccess
    {
        get => _isGettingAccess;
        set => SetProperty(ref _isGettingAccess, value);
    }

    private void UpdateProxyAccess()
    {
        var request = new ProxyAccessRequestDto
        {
            HostType = ProxyHostType.Hostname,
            IpType = ProxyIpType.Sticky,
            ProtocolType = ProxyProtocolType.Http,
            Count = _proxyAccessViewModels.Count,
        };

        var urls = _proxyService
            .GetAccess(request)
            .Select(access => access.Url)
            .ToList();

        for (var i = 0; i < urls.Count; ++i)
        {
            _proxyAccessViewModels[i].Url = urls[i];
        }
    }

    private void OnUpdateStaleDataEvent()
    {
        DispatcherService.InvokeOnUiThreadAsync(async() =>
        {
            await LoadUserProfileFolderViewModels();
            await InitializeViewModels();
        });
    }

    private void OnUserProfileSelected()
    {
        OnPropertyChanged(nameof(FillProxiesIsEnabled));
    }

    private bool _isSelectedAll;
    public bool IsSelectedAll
    {
        get => _isSelectedAll;
        set
        {
            if (SetProperty(ref _isSelectedAll, value))
            {
                SetSelectedAll();
            }
        }
    }

    private void SetSelectedAll()
    {
        var items = ViewModels.Items;
        var folderId = SelectedFolder?.Id ?? 0;
        if (folderId != 0)
        {
            items = items.Where(a => a.UserProfileModel.FolderId == folderId).ToList();
        }

        foreach (var item in items)
        {
            item.IsSelected = IsSelectedAll;
        }
    }

    private string _applingProxy;
    public string ApplingProxy
    {
        get => _applingProxy;
        set => SetProperty(ref _applingProxy, value);
    }

    [RelayCommand]
    public async Task ApplyProxy()
    {                                            
        var models = SelectedProfiles();
        var proxies = await SetProxies(models);

        var proxyCount = proxies.Count;
        var modelCount = models.Count;
        if (proxyCount == 0 || modelCount == 0)
        {
            return;
        }

        await ApplyProxy(proxies, models);
    }


    [RelayCommand]
    public async Task FillProxies()
    {
        var profiles = SelectedProfiles();
        if (profiles.Count == 0)
        {
            return;
        }

        var proxyUrls = GetProxyAccess(profiles.Count);
        if (proxyUrls.Length == 0)
        {
            PurchaseMessage();
            return;
        }

        var proxies = ParseProxiesSettings(proxyUrls);

        await ApplyProxy(proxies, profiles);

        IsSelectedAll = false;
        OnPropertyChanged(nameof(IsSelectedAll));
    }

    private async Task ApplyProxy(List<IProxySettings> proxies, List<UserProxySettingViewModel> models)
    {
        if (proxies.Count == 1)
        {
            for (var i = 0; i < models.Count; ++i)
            {
                await ApplyProxy(proxies[0], models[i]);
            }
        }
        else
        {
            var minCount = Math.Min(proxies.Count, models.Count);

            for (var i = 0; i < minCount; ++i)
            {
                await ApplyProxy(proxies[i], models[i]);
            }
        }
    }

    private async Task ApplyProxy(IProxySettings proxySettings, UserProxySettingViewModel model)
    {
        if (proxySettings != null)
        {
            model.Host = proxySettings.Host;
            model.Port = "" + proxySettings.Port;
            model.UserName = proxySettings.UserName;
            model.Password = proxySettings.Password;
        }
        model.SetProfile();
        await Task.Run(()=>_userProfileService.Save(model.UserProfile));
        //EventAggregator
        //    .GetEvent<OpenUserProfileEvent>()
        //    .Publish(new UserProfileEventArgs(model._userProfile));

    }

    private async Task<List<IProxySettings>> SetProxies(List<UserProxySettingViewModel>? models = null)
    {
        if (string.IsNullOrWhiteSpace(_applingProxy))
        {
            //List<IProxySettings> returned = [];
            if (models != null)
            {
                foreach (var model in models)
                {
                    if (model.UserProfile.Proxy.Host != model.Host ||
                        (model.Port.HasAny() && int.TryParse(model.Port,out int port) && port!= model.UserProfile.Proxy.Port) ||
                        model.UserProfile.Proxy.UserName != model.UserName ||
                        model.UserProfile.Proxy.Password != model.Password)
                        await ApplyProxy(null, model);
                }
            }

            return [];
        }

        var applingProxyList = ApplingProxy.Split(
            [Environment.NewLine],
            StringSplitOptions.RemoveEmptyEntries);

        var proxies = ParseProxiesSettings(applingProxyList);
        return proxies;
    }

    private static List<IProxySettings> ParseProxiesSettings(string[] proxyList)
    {
        var proxies = new List<IProxySettings>();
        foreach (var item in proxyList)
        {
            if (!ParseProxySettings(item, out var proxy))
            {
                continue;
            }
            proxies.Add(proxy);
        }
        return proxies;
    }

    private static bool ParseProxySettings(string applingProxy, out ProxySettings proxy)
    {
        proxy = new ProxySettings();

        var applingProxies = applingProxy
            .StripPrefix("http://")
            .StripPrefix("https://")
            .Split(':');
        if (applingProxies.Length != 4)
        {
            ErrorMessage("Not a valid string");
            return false;
        }

        var portStr = applingProxies[1];
        var isValidPort = Int32.TryParse(portStr, out var port);
        if (!isValidPort && !string.IsNullOrWhiteSpace(portStr))
        {
            ErrorMessage("Port cann't be text");
            return false;
        }

        proxy.Port = port;
        proxy.Host = applingProxies[0];
        proxy.UserName = applingProxies[2];
        proxy.Password = applingProxies[3];

        return true;
    }

    public bool FillProxiesIsEnabled
    {
        get
        {
            if (_viewModels == null)
            {
                return false;
            }

            return _viewModels.Any(viewModel => viewModel.IsSelected);
        }
    }

    private List<UserProxySettingViewModel> SelectedProfiles()
    {
        var models = new List<UserProxySettingViewModel>();
        foreach (var item in ViewModels.Items)
        {
            if (!item.IsSelected)
            {
                continue;
            }
            models.Add(item);
        }
        return models;
    }

    private static async void ErrorMessage(string message)
    {
        await MesageBoxHelper.ShowErrorAsync("Warning", message);
    }
    private async void PurchaseMessage()
    {
        if(await MesageBoxHelper.ShowAsync("No Proxy Credit","You have no proxy to set. Purchase them on Proxy Credit tab"))
        {
            var args = new ChangeSelectedTabIndexEventArgs() { SelectedIndex = 2 };
            EventAggregator
                .GetEvent<ChangeSelectedTabIndexEvent>()
                .Publish(args);
        }
    }

    bool _initViewModels;
    private ObservableCollectionView<UserProxySettingViewModel> _viewModels;
    public ObservableCollectionView<UserProxySettingViewModel> ViewModels
    {
        get
        {
            if ((_viewModels == null || _initViewModels) && _mapping != null)
            {
                _initViewModels = false;
                _viewModels = new ObservableCollectionView<UserProxySettingViewModel>(_mapping)
                {
                    TrackItemChanges = true,
                    Order = profile => profile.UserProfileTitle
                };

                if (SelectedFolder == null || SelectedFolder.Id == 0)
                {
                    ViewModels.Filter = null;
                }
                else
                {
                    ViewModels.Filter = folder => folder.UserProfileModel.FolderId == SelectedFolder?.Id;
                }

                SelectedCount = 0;
                OnPropertyChanged(nameof(SelectedCount));

                _mapping.CollectionChanged -= OnViewModelChange;
                _mapping.CollectionChanged += OnViewModelChange;
                InitPaginator();

                //SelectedFolder = _folderViewModels.Items.First();
            }
            return _viewModels;
        }
    }

    bool _initFolderViewModels;
    private ObservableCollectionView<ProfileFolderViewModel> _folderViewModels;
    public ObservableCollectionView<ProfileFolderViewModel> FolderViewModels
    {
        get
        {
            if ((_folderViewModels == null || _initFolderViewModels) && _folderMapping != null)
            {
                _initFolderViewModels = false;

                var items = _folderMapping
                    .OrderBy(a => a.Title)
                    .ToList();

                items.Insert(0, new ProfileFolderViewModel(0, "All profiles"));

                _folderViewModels = new ObservableCollectionView<ProfileFolderViewModel>(items)
                {
                    TrackItemChanges = true,
                    Ascending = true,
                    Order = folder => folder.Title
                };

                //SetFilter();
            }

            return _folderViewModels;
        }
    }      
    private ProfileFolderViewModel _selectedFolder;
    public ProfileFolderViewModel SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (SetProperty(ref _selectedFolder, value) && ViewModels != null)
            {
                IsSelectedAll = false;
                OnPropertyChanged(nameof(IsSelectedAll));
                DispatcherService.InvokeOnUiThreadAsync(InitializeViewModels);
            }
        }
    }


    private void SetFilter()
    {
        if (FolderId != 0)
        {
            SelectedFolder = _folderViewModels.Items.First(a => a.Id == FolderId);
        }
        else
        {
            SelectedFolder = _folderViewModels.Items.First(a => a.Id == 0);
        }

        OnPropertyChanged(nameof(SelectedFolder));
    }

    private int _folderId;
    public int FolderId
    {
        get => _folderId;
        set
        {
            if (SetProperty(ref _folderId, value))
            {
                SetFilter();
            }
        }
    }

    private void InitPaginator()
    {
        PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
        ViewModels.Offset = PaginatorViewModel.Skip;
        ViewModels.Limit = PaginatorViewModel.OnPageItems;
        TotalCount = PaginatorViewModel.TotalCount;
        //PaginatorViewModel.PageIndex = 0;  //TODO: update on folder change
    }

    private string[] GetProxyAccess(int count)
    {
        var request = new ProxyAccessRequestDto
        {
            HostType = ProxyHostType.Hostname,
            IpType = ProxyIpType.Sticky,
            ProtocolType = ProxyProtocolType.Http,
            Count = count,
        };

        var urls = _proxyService
            .GetAccess(request)
            .Select(access => access.Url)
            .ToArray();

        return urls;
    }

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

    private void OnChangePage(object sendner, EventArgs args)
    {
        ViewModels.Offset = PaginatorViewModel.Skip;
    }

    private void OnViewModelChange(object sendner, EventArgs args)
    {
        var count = _viewModels.Items.Count;
        PaginatorViewModel.TotalCount = count;
        TotalCount = count;
        SelectedCount = _viewModels.Items.Count(a => a.IsSelected);
        HasSelectedItems = SelectedCount > 0;

        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(SelectedCount));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(FillProxiesIsEnabled));
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

    private bool _showCustomizeProxies;
    public bool ShowCustomizeProxies
    {
        get => _showCustomizeProxies;
        set
        {
            SetProperty(ref _showCustomizeProxies, value);
        }
    }

    [RelayCommand]
    private void UnselectItems()
    {
        IsSelectedAll = false;
        foreach (var model in ViewModels.Items)
        {
            model.IsSelected = false;
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var model in ViewModels)
        {
            model.IsSelected = true;
        }
    }
    [RelayCommand]
    private void SelectAllFromFolder()
    {
        IsSelectedAll = true;
    }

    [RelayCommand]
    private void ChangeProxies()
    {
        ShowCustomizeProxies = true;
    }

    [RelayCommand]
    private void HideCustomizeProxies()
    {
        ShowCustomizeProxies = false;
    }

    private List<UserProxySettingViewModel> _selectedProxySetting;
    public List<UserProxySettingViewModel> SelectedProxySetting
    {
        get => _selectedProxySetting;
        set => SetProperty(ref _selectedProxySetting, value);
    }

    private void OnSelectedChanged(SelectedUserProxySettingEventArgs args)
    {
        SelectedCount = ViewModels.Items.Count(setting => setting.IsSelected);
        if (args.OpenChangeProxies)
        {
            ShowCustomizeProxies = true;
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
                HasSelectedItems = _selectedCount > 0;
                OnPropertyChanged(nameof(HasSelectedItems));
            }
        }
    }

    private bool _hasSelectedItems;
    public bool HasSelectedItems
    {
        get => _hasSelectedItems;
        set => SetProperty(ref _hasSelectedItems, value);
    }
}
