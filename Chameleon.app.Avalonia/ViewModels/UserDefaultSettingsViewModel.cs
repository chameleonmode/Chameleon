using Chameleon.Common.Helpers;
using Chameleon.Common.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Chameleon.Interfaces.Settings;
using Chameleon.lib.Common;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.WebBrowser.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class UserDefaultSettingViewModel : ViewModelObjectBase {
	BrowserSettingDto _userDefaultSetting;
	public UserDefaultSettingViewModel(
			BrowserSettingDto userDefaultSetting)
	{
		_defaultUrl = userDefaultSetting.DefaultUrl;
	}


	private bool _hasChanged;

	public bool HasChanged {
		get { return _hasChanged; }
		set { _hasChanged = value; }
	}

	public string _defaultUrl;
	public string DefaultUrl {
		get => _defaultUrl;
		set {
			if (SetProperty(ref _defaultUrl, value)) {
				_hasChanged = true;
			}
		}
	}

	private bool _isChecked;
	public bool IsChecked {
		get => _isChecked;
		set {
			if (SetProperty(ref _isChecked, value)) {
				ChangeSelected();
			}
		}
	}

	private void ChangeSelected()
	{
		EventAggregator
								.GetEvent<SelectedUserDefaultSettingEvent>()
								.Publish(new SelectedUserDefaultSettingEventArgs(_isChecked));
	}

	[RelayCommand]
	public async Task SaveUrlFromViewText()
	{
		if (string.IsNullOrWhiteSpace(DefaultUrl)) {
			return;
		}

		HasChanged = false;

		_userDefaultSetting.DefaultUrl = DefaultUrl;
		await BrowserSettingsRepo.Instance.Create(_userDefaultSetting);
	}

	[RelayCommand]
	public async Task DeleteDefaultSettings()
	{
		_ = await BrowserSettingsRepo.Instance.Delete(_userDefaultSetting.id);
		ChangeSelected();
	}
}

public partial class UserDefaultSettingsViewModel
			 : ViewModelObjectBase {
	private const char BulkAddSeparator = ',';
	private readonly ReadOnlyObservableCollection<UserDefaultSettingViewModel> settings;
	public ObservableCollection<UserDefaultSettingViewModel> ViewModels { get; }

	private int _selectedCount;
	public int SelectedCount {
		get => _selectedCount;
		set {
			if (SetProperty(ref _selectedCount, value)) {
				OnPropertyChanged(nameof(HasSelectedItems));
			}
		}
	}

	public bool HasSelectedItems => SelectedCount > 0;

	[ObservableProperty]
	BrowserDefaultLaunchSettings thesebrowserDefaultLaunchSettings;

	public UserDefaultSettingsViewModel() : base("Default Browser Settings")
	{
		_ = BrowserSettingsRepo.Instance.ObservableCache
			.Connect()
			.Transform(p=> new UserDefaultSettingViewModel(p))
			.Bind(out settings)
			.Subscribe();

		EventAggregator
			 .GetEvent<SelectedUserDefaultSettingEvent>()
			 .Subscribe(_ => OnSelectedChanged());
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (!Loaded) {
			ThesebrowserDefaultLaunchSettings = await BrowserDefaultLaunchSettings.Instance();
		}
	}

	//[RelayCommand]
	//private async Task BulkAddPages()
	//{
	//	var result = await _bulkAddPagesPopupViewModel.ShowAsync();
	//	if (result == IContentDialogResult.Primary) {
	//		await AddPages(_bulkAddPagesPopupViewModel.Urls);
	//	} else {
	//		_bulkAddPagesPopupViewModel.Urls = null;
	//	}
	//}

	//private async Task AddPages(string urls)
	//{
	//	if (!string.IsNullOrEmpty(urls)) {
	//		await AddPages(urls.Split(BulkAddSeparator, StringSplitOptions.RemoveEmptyEntries));
	//	}
	//	_bulkAddPagesPopupViewModel.Urls = null;
	//}

	private async Task AddPages(string[] urls)
	{
		for (int i = 0; i < urls.Length; i++) {
			if (i < ViewModels.Count) {
				ViewModels[i].DefaultUrl = string.IsNullOrWhiteSpace(urls[i]) ? null : urls[i].Trim();
			} else {
				await CreateNewDefaultSettings();
				ViewModels.Last().DefaultUrl = string.IsNullOrWhiteSpace(urls[i]) ? null : urls[i].Trim();
			}
		}

		OnPropertyChanged(nameof(ViewModels));
		Save();
	}
	[RelayCommand]
	private void UnselectItems()
	{
		foreach (var setting in ViewModels) {
			setting.IsChecked = false;
		}
	}

	[RelayCommand]
	private async Task RemoveSelectedItems()
	{
		var _selectedDefaultSetting = ViewModels.Where(v => v.IsChecked);
		if (_selectedDefaultSetting == null || _selectedDefaultSetting.Count() == 0) {
			return;
		}

		foreach (var setting in _selectedDefaultSetting) {
			await setting.DeleteDefaultSettings();
		}
		OnSelectedChanged();
	}

	[RelayCommand]
	private async Task CreateNewDefaultSettings()
	{
		_ = await BrowserSettingsRepo.Instance.Create(new BrowserSettingDto());
	}

	[RelayCommand]
	private void Save()
	{
		foreach (var viewModel in ViewModels.Where(m => m.HasChanged)) {
			viewModel.SaveUrlFromViewText();
		}

		IoC.SetJsonValue(ViewModels.Select(v => v.DefaultUrl).ToArray(), "DefaultHomePageSettings");
	}

	[RelayCommand]
	private async Task SaveLaunchSettings()
	{
		await BrowserDefaultLaunchSettings.Save();
		IoC.SetJsonValue(new EmulationOptions {
			DisableWebRTC = ThesebrowserDefaultLaunchSettings.Options.DisableWebRTC,
			SpoofClientRects = ThesebrowserDefaultLaunchSettings.Options.SpoofClientRects,
			SpoofFontFingerprint = ThesebrowserDefaultLaunchSettings.Options.SpoofFontFingerprint,
			SpoofCanvasFingerprint = ThesebrowserDefaultLaunchSettings.Options.SpoofCanvasFingerprint,
			SpoofWebGLFingerprint = ThesebrowserDefaultLaunchSettings.Options.SpoofWebGLFingerprint,
			SpoofGeoLocation = ThesebrowserDefaultLaunchSettings.Options.SpoofGeoLocation,
			AutoTimezone = ThesebrowserDefaultLaunchSettings.Options.AutoTimezone
		}, nameof(EmulationOptions));
	}

	private void OnSelectedChanged()
	{
		var _selectedDefaultSetting = ViewModels.Where(v => v.IsChecked);
		if (_selectedDefaultSetting == null) {
			return;
		}

		SelectedCount = _selectedDefaultSetting.Count();
	}
}

public partial class BrowserDefaultLaunchSettings : ObservableObject, IBrowserDefaultLaunchSettings {
	public const string Filename = "defaultBrowserSettings.json";
	[JsonIgnore]
	private static readonly JsonSerializerOptions options = new JsonSerializerOptions {
		Converters =
			{
						new DynamicJsonConverter<Options, IOptions>(),
						new DynamicJsonConverter<Protectkbfingerprint, IProtectkbfingerprint>()
				}
	};
	public Config Config { get; set; }
	public object[] Excluded { get; set; }
	public Headers Headers { get; set; }
	public object[] IpRules { get; set; }
	public BrowserProfile Profile { get; set; }
	public IOptions Options { get; set; } = new Options();
	public Whitelist Whitelist { get; set; }

	//make singleton
	private static BrowserDefaultLaunchSettings instance;
	//private BrowserDefaultLaunchSettings() { }
	public static async Task<BrowserDefaultLaunchSettings> Instance()
	{

		if (instance == null) {
			instance = new BrowserDefaultLaunchSettings();
			// Call the async initialization method
			await instance.InitializeAsync();
		}
		return instance;
	}


	// Async initialization method
	private async Task InitializeAsync()
	{
		// Load settings from a file or a remote source
		// var settings = await LoadSettingsFromFileAsync("settings.json");
		var json = await ConfigHelper.ReadFromAppDir(Filename);
		if (json == null)
			return;

		var settings = JsonSerializer.Deserialize<BrowserDefaultLaunchSettings>(json, options);
		// Apply settings to properties
		// this.Config = settings.Config;
		// this.Excluded = settings.Excluded;
		// this.Headers = settings.Headers;
		// this.IpRules = settings.IpRules;
		// this.Profile = settings.Profile;
		this.Options = settings.Options;
		// this.Whitelist = settings.Whitelist;
	}

	public static async Task Save()
	{
		// Save settings to a file or a remote source
		await ConfigHelper.WriteToAppDir(Filename, JsonSerializer.Serialize(await Instance(), options));
	}
}

public partial class Options : ObservableObject, IOptions {
	[ObservableProperty]
	private bool cookieNotPersistent;
	[ObservableProperty]
	private string cookiePolicy;
	[ObservableProperty]
	private bool blockMediaDevices;
	[ObservableProperty]
	private bool blockCSSExfil;
	[ObservableProperty]
	private bool disableWebRTC = true;
	[ObservableProperty]
	private bool firstPartyIsolate;
	[ObservableProperty]
	private bool limitHistory;
	[ObservableProperty]
	private IProtectkbfingerprint protectKBFingerprint = new Protectkbfingerprint();
	[ObservableProperty]
	private bool protectWinName;
	[ObservableProperty]
	private bool resistFingerprinting;
	[ObservableProperty]
	private string screenSize;
	[ObservableProperty]
	private bool spoofAudioContext = true;
	[ObservableProperty]
	private bool spoofClientRects = true;
	[ObservableProperty]
	private bool spoofFontFingerprint = true;
	[ObservableProperty]
	private bool spoofMediaDevices = true;
	[ObservableProperty]
	private string timeZone;
	[ObservableProperty]
	private bool autoTimezone = true;
	[ObservableProperty]
	private string trackingProtectionMode;
	[ObservableProperty]
	private string webRTCPolicy;
	[ObservableProperty]
	private string webSockets;

	[ObservableProperty]
	private bool spoofCanvasFingerprint = true;
	[ObservableProperty]
	private bool spoofWebGLFingerprint = true;
	[ObservableProperty]
	private bool spoofWebGPUFingerprint = true;


	[ObservableProperty]
	private bool spoofGeoLocation = true;
}

public partial class Protectkbfingerprint : ObservableObject, IProtectkbfingerprint {
	[ObservableProperty]
	private bool enabled;
	[ObservableProperty]
	int delay = 1;
}
