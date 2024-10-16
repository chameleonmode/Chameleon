using System.Collections.ObjectModel;

using AutoMapper;

using Chameleon.App.Shared.Proxies;
using Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.App.Prospector;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Proxies;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.WordPress;
using Chameleon.Interfaces.YouTube;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.ViewModels;

public class WebBrowserSettings : IWebBrowserSettings {
	public WebBrowserSettings()
	{
		Canvas = 0.1M;
	}

	public bool WebRTC { get; set; }
	public bool WebGL { get; set; }
	public bool Tracking { get; set; }
	public bool Flash { get; set; }

	private decimal _canvas;
	public decimal Canvas {
		get => _canvas;
		set {
			if (value < 0) {
				//throw new ArgumentOutOfRangeException(nameof(Canvas), "value should be > 0");
				return;
			}
			_canvas = value;
		}
	}

	private int? _userAgentId;
	public int? UserAgentId {
		get => UserAgent?.Id ?? _userAgentId;
		set {
			if (value == null) {
				UserAgent = null;
				_userAgentId = null;
				return;
			}

			if (value.Value <= 0) {
				//throw new ArgumentOutOfRangeException(nameof(UserAgentId), "value should be > 0");
				return;
			}

			if (UserAgent != null && UserAgent.Id != value.Value) {
				UserAgent = null;
			}
			_userAgentId = value;
		}
	}

	public IWebBrowserUserAgent UserAgent { get; set; }
}
public class ProxySettings : IProxySettings {
	private string _host = string.Empty;
	public string Host {
		get => _host;
		set => _host = value?.Trim() ?? string.Empty;
	}

	public string HostForRequest { get => HostConverter.GetHostForRequest(Host); }

	public const int DefaultPort = 80;
	private int _port = DefaultPort;
	public int Port {
		get => _port;
		set {
			if (value < 0 || value >= 65535) {
				value = 0;
				// throw new ArgumentOutOfRangeException();
			}
			_port = value;
		}
	}

	private string _userName = string.Empty;
	public string UserName {
		get => _userName;
		set => _userName = value?.Trim() ?? string.Empty;
	}

	private string _password = string.Empty;
	public string Password {
		get => _password;
		set => _password = value?.Trim() ?? string.Empty;
	}

	public bool CanUse => Host.Length > 0;
	public bool HasUserName => UserName.Length > 0;
	public string Server => CanUse ? $"{HostForRequest}:{Port}" : string.Empty;

	public string ServerForRequest => CanUse ? $"http://{Server}" : string.Empty;
}
public class UserProfile : IUserProfile {
	private int _id;
	public int Id {
		get => _id;
		set {
			if (value <= 0) {
				return;
				//throw new ArgumentException();
			}
			_id = value;
		}
	}

	public string? Title { get; set; }
	public bool IsFavourite { get; set; }

	private int? _folderId;
	public int? FolderId {
		get => _folderId;
		set {
			if (value <= 0) {
				// throw new ArgumentException();
				return;
			}
			_folderId = value;
		}
	}
	private long? _creatorUserId;
	public long? CreatorUserId {
		get => _creatorUserId;
		set {
			if (value <= 0) {
				return;
				//throw new ArgumentException();
			}
			_creatorUserId = value;
		}
	}

	private string _notes = string.Empty;
	public string Notes {
		get => _notes;
		set => _notes = value ?? string.Empty;
	}
	private double? _limitCache;
	public double? LimitCache {
		get => _limitCache ?? 100;
		set {
			if (value <= 0) {
				return;
				//throw new ArgumentException();
			}
			_limitCache = value;
		}
	}

	private IProxySettings _proxy = new ProxySettings();
	public IProxySettings Proxy {
		get => _proxy;
		set => _proxy = value ?? new ProxySettings();
	}

	private IWebBrowserSettings _webBrowser = new WebBrowserSettings();
	public IWebBrowserSettings WebBrowser {
		get => _webBrowser;
		set => _webBrowser = value ?? new WebBrowserSettings();
	}

	public IList<IUserProfileBusiness> Businesses { get; }
			= new List<IUserProfileBusiness>();

	public IList<IUserProfileLogin> Logins { get; }
			= new List<IUserProfileLogin>();

	public IList<IUserProfilePerson> Persons { get; }
			= new List<IUserProfilePerson>();

	public IList<IUserProfileAddress> Addresses { get; }
			= new List<IUserProfileAddress>();

	public string[] PermissionNames { get; set; } = [];

	public bool HasPermission(string permissionName)
	{
		return PermissionNames.Contains(permissionName);
	}

	public bool Navigated { get; set; }
	public string IsChromeRunning { get; set; } = "False";
	public string IsBraveRunning { get; set; } = "False";
	public string IsFFRunning { get; set; } = "False";

	public Dictionary<SystemBrowserType, ISysBrowserInstance?> SBI { get; set; } = new Dictionary<SystemBrowserType, ISysBrowserInstance?>(){
			{ SystemBrowserType.Chrome, null },
			{ SystemBrowserType.Firefox, null },
			{ SystemBrowserType.Brave, null }
		};
	public IYouTubeSettings YoutubeSettings { get; set; }
	public IWordPressSettings WordPressSettings { get; set; }
	public IList<IUserProfileOutReachRss> OutReachRsses { get; }
	public IList<IUserProfileProspectorBlogsOfInterest> ProspectorBlogsOfInterest { get; }
}

public partial class UserProxySettingViewModel
			 : ViewModelObjectBase {
	public static string InProject => "Profile";

	private readonly IUserProfile _userProfile;
	private readonly IEventAggregator _eventAggregator;

	[ObservableProperty]
	private string? _host;
	[ObservableProperty]
	private string? _userName;
	[ObservableProperty]
	private string? _password;
	[ObservableProperty]
	private string? _port;
	[ObservableProperty]
	private IUserProfile _userProfileModel;
	public UserProxySettingViewModel(
				IUserProfile userProfile,
				IEventAggregator eventAggregator
				)
	{
		_userProfile = userProfile;
		_eventAggregator = eventAggregator;

		_userProfileModel = userProfile;
		_host = _userProfileModel.Proxy.Host;
		_port = "" + _userProfileModel.Proxy.Port;
		_userName = _userProfileModel.Proxy.UserName;
		_password = _userProfileModel.Proxy.Password;
	}

	public string UserProfileTitle => _userProfile.Title ?? "<Title>";

	private bool _isSelected;
	public bool IsSelected {
		get => _isSelected;
		set {
			SetProperty(ref _isSelected, value);

			if (!_openChangeProxies && value)
				ClickIconChangeProxies();

			ChangeSelected();
		}
	}

	private void ChangeSelected()
	{
		_eventAggregator
								.GetEvent<SelectedChangeUserProfileEvent>()
								.Publish(new SelectedUserProfileEventArgs(_userProfile, IsSelected));

		_eventAggregator
								.GetEvent<SelectedUserProxySettingEvent>()
								.Publish(new SelectedUserProxySettingEventArgs(IsSelected, _openChangeProxies));
		_openChangeProxies = false;
	}

	private string _code;
	public string Code {
		get {
			if (string.IsNullOrEmpty(_code)) {
				if (UserProfileTitle.Is()) {
					var list = UserProfileTitle.Split(" ")
							.Select(a => a.Trim().ToUpper()[0])
							.ToList();

					if (list.Count > 2) {
						list = list.Take(2).ToList();
					}

					_code = string.Join("", list);
				} else {
					_code = "XX";
				}
			}

			return _code;
		}
	}

	public void SetProfile()
	{
		var host = Host;
		var port = int.TryParse(Port, out var po) ? po : _userProfile.Proxy.Port;
		var userName = UserName;
		var password = Password;

		var profileProxy = _userProfile.Proxy;
		profileProxy.Host = host;
		profileProxy.Port = port;
		profileProxy.UserName = userName;
		profileProxy.Password = password;

		var modelProxy = UserProfileModel.Proxy;
		modelProxy.Host = host;
		modelProxy.Port = port;
		modelProxy.UserName = userName;
		modelProxy.Password = password;
	}

	private bool _openChangeProxies = false;
	[RelayCommand]
	private void ClickIconChangeProxies()
	{
		_openChangeProxies = true;
		IsSelected = true;
	}
}
public class ProfileFolderViewModel
    : ViewModelObjectBase {
    public ProfileFolderViewModel(int id, string title)
		: base(title)
    {
        Id = id;
    }

    private int _id;
    public int Id {
		get => _id;
		set => SetProperty(ref _id, value);
	}
}

public partial class UserProxySettingsViewModel
       : ViewModelObjectBase
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
        _proxyAccessViewModels = proxyAccessViewModels;
        _proxyAccessViewModels.AddItems(CountProxies);
        _userProfileFolderService = userProfileFolderService;
		_userProfileFolderService = userProfileFolderService;

		//EventAggregator
		//    .GetEvent<SavedUserProfileEvent>()
		//    .Subscribe(args => OnUserProfileSaved());

		//EventAggregator
		//  .GetEvent<UpdateStaleDataEvent>()
		//  .Subscribe(() => OnUpdateStaleDataEvent());

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
            userProfiles, userProfile => new UserProxySettingViewModel(userProfile, EventAggregator)
            );

        _initViewModels = true;
        OnPropertyChanged(nameof(ViewModels));
        OnPropertyChanged(nameof(HasSelectedItems));
        OnPropertyChanged(nameof(FillProxiesIsEnabled));

		_ = initializeViewModelsSlim.Release();
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

    private async void UpdateProxyAccessAsync()
    {
        IsGettingAccess = true;
				await Task.Run(UpdateProxyAccess);
				IsGettingAccess = false;
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

    private async void OnUpdateStaleDataEvent()
    {
			await LoadUserProfileFolderViewModels();
			await InitializeViewModels();
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
        await Task.Run(()=>_userProfileService.Save(model.UserProfileModel));
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
                    if (model.UserProfileModel.Proxy.Host != model.Host ||
                        (model.Port.Is() && int.TryParse(model.Port,out int port) && port!= model.UserProfileModel.Proxy.Port) ||
                        model.UserProfileModel.Proxy.UserName != model.UserName ||
                        model.UserProfileModel.Proxy.Password != model.Password)
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
			Toaster.ShowErr($"Not a valid set {applingProxy}");
            return false;
        }

        var portStr = applingProxies[1];
        var isValidPort = Int32.TryParse(portStr, out var port);
        if (!isValidPort && !string.IsNullOrWhiteSpace(portStr))
        {
			Toaster.ShowErr($"Port cann't be text {applingProxy}");
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
    private async void PurchaseMessage()
    {
        if(await Mbox.Show("No Proxy Credit","You have no proxy to set. Purchase them on Proxy Credit tab"))
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
								_ = InitializeViewModels();
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
