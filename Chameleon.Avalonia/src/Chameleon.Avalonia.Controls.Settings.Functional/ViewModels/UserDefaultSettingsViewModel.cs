using Chameleon.CT.Common.Models;
using Chameleon.lib.Common;
using Chameleon.lib.WebBrowser.Models;

namespace Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;

public partial class UserDefaultSettingsViewModel
			 : SubPageViewModelBase
			 , IUserDefaultSettingsViewModel {
	private const char BulkAddSeparator = ',';

	private readonly IUserDefaultSettingsService _userDefaultSettingsService;
	private readonly IBulkAddPagesPopupViewModel _bulkAddPagesPopupViewModel;
	private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

	private ObservableCollection<IUserDefaultSetting, UserDefaultSettingViewModel> _mapping;
	private List<UserDefaultSettingViewModel> _selectedDefaultSetting;

	private ObservableCollectionView<UserDefaultSettingViewModel> _viewModels;
	public ObservableCollectionView<UserDefaultSettingViewModel> ViewModels {
		get {
			if (_viewModels == null && _mapping != null) {
				_viewModels = new ObservableCollectionView<UserDefaultSettingViewModel>(_mapping);
			}
			return _viewModels;
		}
	}

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
	BrowserDefaultLaunchSettings browserDefaultLaunchSettings;

	public UserDefaultSettingsViewModel(
			IUserDefaultSettingsService userDefaultsSettingsService,
			IBulkAddPagesPopupViewModel bulkAddPagesPopupView,
			IUserDefaultSettingsService userDefaultSettingsService
			)
	{
		Title = "Default Browser Settings";

		_userDefaultSettingsService = userDefaultSettingsService;
		_userDefaultsSettingsService = userDefaultsSettingsService;
		_bulkAddPagesPopupViewModel = bulkAddPagesPopupView;

		EventAggregator
			 .GetEvent<SelectedUserDefaultSettingEvent>()
			 .Subscribe(_ => OnSelectedChanged());
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (!Loaded) {
			var userSettings = _userDefaultsSettingsService.GetAll();

			_mapping = new ObservableCollection<IUserDefaultSetting, UserDefaultSettingViewModel>(
					userSettings, userSetting => new UserDefaultSettingViewModel(EventAggregator, userSetting, _userDefaultsSettingsService));

			BrowserDefaultLaunchSettings = await BrowserDefaultLaunchSettings.Instance();
			OnPropertyChanged(nameof(ViewModels));
		}
	}


	[RelayCommand]
	private async Task BulkAddPages()
	{
		var result = await _bulkAddPagesPopupViewModel.ShowAsync();
		if (result == IContentDialogResult.Primary) {
			await AddPages(_bulkAddPagesPopupViewModel.Urls);
		} else {
			_bulkAddPagesPopupViewModel.Urls = null;
		}
	}

	private async Task AddPages(string urls)
	{
		if (!string.IsNullOrEmpty(urls)) {
			await AddPages(urls.Split(BulkAddSeparator, StringSplitOptions.RemoveEmptyEntries));
		}
		_bulkAddPagesPopupViewModel.Urls = null;
	}

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
		foreach (var setting in _mapping) {
			setting.IsChecked = false;
		}
	}

	[RelayCommand]
	private void RemoveSelectedItems()
	{
		if (_selectedDefaultSetting == null || _selectedDefaultSetting.Count == 0) {
			return;
		}

		foreach (var setting in _selectedDefaultSetting) {
			setting.DeleteDefaultSettings();
		}
		OnSelectedChanged();
	}

	[RelayCommand]
	private async Task CreateNewDefaultSettings()
	{
		_ = await Task.Run(_userDefaultSettingsService.Create);
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
			DisableWebRTC = BrowserDefaultLaunchSettings.Options.DisableWebRTC,
			SpoofClientRects = BrowserDefaultLaunchSettings.Options.SpoofClientRects,
			SpoofFontFingerprint = BrowserDefaultLaunchSettings.Options.SpoofFontFingerprint,
			SpoofCanvasFingerprint = BrowserDefaultLaunchSettings.Options.SpoofCanvasFingerprint,
			SpoofWebGLFingerprint = BrowserDefaultLaunchSettings.Options.SpoofWebGLFingerprint,
			SpoofGeoLocation = BrowserDefaultLaunchSettings.Options.SpoofGeoLocation,
			AutoTimezone = BrowserDefaultLaunchSettings.Options.AutoTimezone
		}, nameof(EmulationOptions));
	}

	private void OnSelectedChanged()
	{
		_selectedDefaultSetting = _mapping
				.Where(setting => setting.IsChecked)
				.ToList();

		SelectedCount = _selectedDefaultSetting.Count;
	}
}
