using AutoMapper;
using Chameleon.App.Shared.Proxies;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Avalonia.Prism.Interfaces.MessageBox;
using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.MessageBox;
using Chameleon.Interfaces.Proxies;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Drawing;
using Chameleon.Core.Extensions;
using Chameleon.Avalonia.Prism.Module.Collections;
using Chameleon.Avalonia.Controls.Settings.ViewModels.ProxyAccess;
using Chameleon.CT.Common.Base;
using Chameleon.Common.Base;
using Chameleon.Interfaces.Dialogs;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class UserProxySettingsViewModel
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

        EventAggregator
            .GetEvent<SavedUserProfileEvent>()
            .Subscribe(args => OnUserProfileSaved());

        EventAggregator
          .GetEvent<UpdateStaleDataEvent>()
          .Subscribe(() => OnUserProfileSaved());

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
           .Subscribe(ChangeFoldersCollection);

        EventAggregator
            .GetEvent<RenameFolderEvent>()
            .Subscribe(args => OnRenameFolder(args.FolderId, args.Title));

        ApplyProxyCommand = new DelegateCommand(ApplyProxy);
        FillProxiesCommand = new DelegateCommand(FillProxies);
        UnselectItemsCommand = new DelegateCommand(UnselectItems);
        ChangeProxiesCommand = new DelegateCommand(ChangeProxies);
        HideCustomizeProxiesCommand = new DelegateCommand(HideCustomizeProxies);
        InitializeCountriesAsync();
    }
    public override Task LoadAsync()
    {
        if(!base.Loaded)
            Load();
        return base.LoadAsync();
    }
    private void OnRenameFolder(int folderId, string title)
    {
        var item = FolderViewModels.First(a => a.Id == folderId);
        item.Title = title;
    }

    private void ChangeFoldersCollection()
    {
        LoadUserProfileFolderViewModels();
    }

    private void Load()
    {
        InitializeViewModels();
        LoadUserProfileFolderViewModels();
       
    }

    public AsyncCollectionViewModel<IProxyCountry> Countries { get; private set; }

    private void InitializeCountriesAsync()
    {
        Countries = new AsyncCollectionViewModel<IProxyCountry>(GetCountries, true);
        Countries.Load();
    }

    private IList<IProxyCountry> GetCountries()
    {
        var countries = _proxyService.GetCountries();
        DispatcherService.InvokeOnUiThread(() => Country = countries.FirstOrDefault());
        return countries;
    }

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

    private void OnUserProfileSaved()
    {
        InitializeViewModels();
    }

    private void OnUserProfileSelected()
    {
        OnPropertyChanged(nameof(FillProxiesIsEnabled));
    }

    private void InitializeViewModels()
    {
        var userProfiles = _userProfileService.GetAll();

        _mapping = new ObservableCollection<IUserProfile, UserProxySettingViewModel>(
            userProfiles, userProfile => new UserProxySettingViewModel(_mapper, userProfile, EventAggregator)
            );

        InitViewModels = true;
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasSelectedItems));
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

    public DelegateCommand ApplyProxyCommand { get; }

    public void ApplyProxy()
    {
        var proxies = SetProxies();
        var models = SelectedProfiles();

        var proxyCount = proxies.Count;
        var modelCount = models.Count;
        if (proxyCount == 0 || modelCount == 0)
        {
            return;
        }

        if (proxyCount == 1)
        {
            ApplyProxy(proxies[0], models);
            return;
        }

        ApplyProxy(proxies, models);
    }

    private void ApplyProxy(List<IProxySettings> proxies, List<UserProxySettingViewModel> models)
    {
        var proxyCount = proxies.Count;
        var modelCount = models.Count;
        var minCount = proxyCount >= modelCount
            ? modelCount
            : proxyCount;

        for (var i = 0; i < minCount; ++i)
        {
            ApplyProxy(proxies[i], models[i]);
        }
    }

    private void ApplyProxy(IProxySettings proxySettings, UserProxySettingViewModel model)
    {
        model.SetProfile(proxySettings);
        _userProfileService.Save(model._userProfile);
    }

    private void ApplyProxy(IProxySettings proxySettings, List<UserProxySettingViewModel> models)
    {
        foreach (var model in models)
        {
            model.SetProfile(proxySettings);
            _userProfileService.Save(model._userProfile);
        }
    }

    private List<IProxySettings> SetProxies()
    {
        if (string.IsNullOrWhiteSpace(_applingProxy))
        {
            return new List<IProxySettings>();
        }

        var applingProxyList = _applingProxy.Split(new[]
        {
                Environment.NewLine
            },
        StringSplitOptions.RemoveEmptyEntries);

        var proxies = ParseProxiesSettings(applingProxyList);
        return proxies;
    }

    private List<IProxySettings> ParseProxiesSettings(string[] proxyList)
    {
        var proxies = new List<IProxySettings>();
        foreach (var item in proxyList)
        {
            if (!ParseProxySettings(item, out var proxy))
            {
                break;
            }
            proxies.Add(proxy);
        }
        return proxies;
    }

    private bool ParseProxySettings(string applingProxy, out ProxySettings proxy)
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

    public DelegateCommand FillProxiesCommand { get; }
    public void FillProxies()
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

        ApplyProxy(proxies, profiles);

        IsSelectedAll = false;
        OnPropertyChanged(nameof(IsSelectedAll));
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

    private void ErrorMessage(string message)
    {
        ContentDialogService.ShowContentDialogAsync(
            ContentDialogButtons.OK,
            message,
            "Warning");
    }

    private async void PurchaseMessage()
    {
        var result = await ContentDialogService.ShowContentDialogAsync( 
            ContentDialogButtons.OKCancel, 
            "You have no proxy to set. Purchase them on Proxy Credit tab", 
            "You have no proxy to set");
        if(result == IContentDialogResult.Primary)
        {
            var args = new ChangeSelectedTabIndexEventArgs() { SelectedIndex = 2 };
            EventAggregator
                .GetEvent<ChangeSelectedTabIndexEvent>()
                .Publish(args);
        }
    }

    private bool InitViewModels = false;
    private ObservableCollectionView<UserProxySettingViewModel> _viewModels;
    public ObservableCollectionView<UserProxySettingViewModel> ViewModels
    {
        get
        {
            if ((_viewModels == null || InitViewModels) && _mapping != null)
            {
                InitViewModels = false;
                _viewModels = new ObservableCollectionView<UserProxySettingViewModel>(_mapping)
                {
                    TrackItemChanges = true,
                    Order = profile => profile.Title
                };

                if (SelectedFolder.Id == 0)
                {
                    ViewModels.Filter = null;
                }
                else
                {
                    ViewModels.Filter = folder => folder.UserProfileModel.FolderId == SelectedFolder.Id;
                }

                SelectedCount = 0;
                OnPropertyChanged(nameof(SelectedCount));

                _mapping.CollectionChanged += OnViewModelChange;
                InitPaginator();
            }
            return _viewModels;
        }
    }

    private ObservableCollectionView<ProfileFolderViewModel> _folderViewModels;
    public ObservableCollectionView<ProfileFolderViewModel> FolderViewModels
    {
        get
        {
            if ((_folderViewModels == null || _folderViewModels.Count == 0 || _initFolderViewModels) && _folderMapping != null)
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

                SetFilter();
            }

            return _folderViewModels;
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

    private bool _initFolderViewModels;
    private void LoadUserProfileFolderViewModels()
    {
        var folders = _userProfileFolderService.GetAll();

        _folderMapping = new ObservableCollection<IUserProfileFolder, ProfileFolderViewModel>(
            folders, folder => new ProfileFolderViewModel(folder.Id, folder.Title));

        _initFolderViewModels = true;
        OnPropertyChanged(nameof(FolderViewModels));
    }

    private void InitPaginator()
    {
        PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
        ViewModels.Offset = PaginatorViewModel.Skip;
        ViewModels.Limit = PaginatorViewModel.OnPageItems;
        TotalCount = PaginatorViewModel.TotalCount;
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

    public DelegateCommand UnselectItemsCommand { get; }
    private void UnselectItems()
    {
        IsSelectedAll = false;
        foreach (var model in ViewModels.Items)
        {
            model.IsSelected = false;
        }
    }

    public DelegateCommand ChangeProxiesCommand { get; }
    private void ChangeProxies()
    {
        ShowCustomizeProxies = true;
    }

    public DelegateCommand HideCustomizeProxiesCommand { get; }
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
