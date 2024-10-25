using System.Text.Json.Serialization;
using System.Text.Json;
using Chameleon.lib.Common;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Common.Constants;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Configuration;
using Chameleon.lib.Common.Models;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class UserDefaultSettingViewModel : ViewModelObjectBase {
	public event Action OnSelectedChanged;
	BrowserSettingDto _userDefaultSetting;
	public UserDefaultSettingViewModel(BrowserSettingDto userDefaultSetting, Action onSelectedChanged)
	{
		OnSelectedChanged = onSelectedChanged;
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
		OnSelectedChanged();
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
			.Transform(p=> new UserDefaultSettingViewModel(p, OnSelectedChanged))
			.Bind(out settings)
			.Subscribe();
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
		this.Options = settings!.Options;
		// this.Whitelist = settings.Whitelist;
	}

	public static async Task Save()
	{
		// Save settings to a file or a remote source
		await ConfigHelper.WriteToAppDir(Filename, JsonSerializer.Serialize(await Instance(), options));
	}

	public static class ConfigHelper {
		private static string? _lastSelectedBrowser;
		public static string? LastSelectedBrowser {
			get => _lastSelectedBrowser ??= GetSetting();
			set => SetSetting(ref _lastSelectedBrowser, value);
		}

		private static int? _lastRunScriptId = null;
		public static int LastRunScriptId {
			get => _lastRunScriptId ??= int.Parse(GetSetting()!);
			set => SetSetting(ref _lastRunScriptId, value);
		}


		private static string? _userScriptsDirectory;
		public static string? UserScriptsDirectory {
			get => _userScriptsDirectory ??= GetSetting();
			set => SetSetting(ref _userScriptsDirectory, value);
		}

		public static bool SetSetting<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, newValue))
				return false;

			field = newValue;

			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings[propertyName].Value = field?.ToString();
			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");

			return true;
		}

		public static string? GetSetting([CallerMemberName] string? propertyName = null)
		{
			return ConfigurationManager.AppSettings[propertyName];
		}

		public static Task WriteToAppDir(string fname, string content)
		{
			var settingsFilePath = Path.Combine(Consts.AppDataLocalDir, fname);
			return File.WriteAllTextAsync(settingsFilePath, content);
		}

		public static Task<string> ReadFromAppDir(string fname)
		{
			var settingsFilePath = Path.Combine(Consts.AppDataLocalDir, fname);
			return !File.Exists(settingsFilePath) ? Task.FromResult(string.Empty) : File.ReadAllTextAsync(settingsFilePath);
		}
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

public interface ISettingsSettings {
	string? CurrentAppTheme { get; set; }
	string? CustomAccentColor { get; set; }
	bool UseCustomAccentColor { get; set; }
	bool AutoLogin { get; set; }
	string CodesverifyApiKey { get; set; }
	string SMSPoolApiKey { get; set; }
	string UserScriptsDirectory { get; set; }
}

public interface IBrowserDefaultLaunchSettings {
	Config Config { get; set; }
	object[] Excluded { get; set; }
	Headers Headers { get; set; }
	object[] IpRules { get; set; }
	BrowserProfile Profile { get; set; }
	IOptions Options { get; set; }
	Whitelist Whitelist { get; set; }
}
public class Config {
	public bool Enabled { get; set; }
	public bool NotificationsEnabled { get; set; }
	public string Theme { get; set; }
	public int ReloadIPStartupDelay { get; set; }
}

public class Headers {
	public bool BlockEtag { get; set; }
	public bool EnableDNT { get; set; }
	public Referer Referer { get; set; }
	public Spoofacceptlang SpoofAcceptLang { get; set; }
	public Spoofip SpoofIP { get; set; }
}

public class Referer {
	public bool Disabled { get; set; }
	public int Xorigin { get; set; }
	public int Trimming { get; set; }
}

public class Spoofacceptlang {
	public bool Enabled { get; set; }
	public string Value { get; set; }
}

public class Spoofip {
	public bool Enabled { get; set; }
	public int Option { get; set; }
	public string RangeFrom { get; set; }
	public string RangeTo { get; set; }
}

public class BrowserProfile {
	public string Selected { get; set; }
	public Interval Interval { get; set; }
	public bool ShowProfileOnIcon { get; set; }
}

public class Interval {
	public int Option { get; set; }
	public int Min { get; set; }
	public int Max { get; set; }
}

public interface IOptions {
	bool CookieNotPersistent { get; set; }
	string CookiePolicy { get; set; }
	bool BlockMediaDevices { get; set; }
	bool BlockCSSExfil { get; set; }
	bool DisableWebRTC { get; set; }
	bool FirstPartyIsolate { get; set; }
	bool LimitHistory { get; set; }
	IProtectkbfingerprint ProtectKBFingerprint { get; set; }
	bool ProtectWinName { get; set; }
	bool ResistFingerprinting { get; set; }
	string ScreenSize { get; set; }
	bool SpoofAudioContext { get; set; }
	bool SpoofClientRects { get; set; }
	bool SpoofFontFingerprint { get; set; }
	bool SpoofMediaDevices { get; set; }
	bool SpoofCanvasFingerprint { get; set; }
	bool SpoofWebGLFingerprint { get; set; }
	bool SpoofWebGPUFingerprint { get; set; }
	bool SpoofGeoLocation { get; set; }
	string TimeZone { get; set; }
	bool AutoTimezone { get; set; }
	string TrackingProtectionMode { get; set; }
	string WebRTCPolicy { get; set; }
	string WebSockets { get; set; }
}

public interface IProtectkbfingerprint {
	bool Enabled { get; set; }
	int Delay { get; set; }
}

public class Whitelist {
	public bool enabledContextMenu { get; set; }
	public string defaultProfile { get; set; }
	public object[] rules { get; set; }
}
