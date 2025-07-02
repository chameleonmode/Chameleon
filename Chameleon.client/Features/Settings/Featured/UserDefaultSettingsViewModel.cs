using Chameleon.client.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Util;
using Chameleon.lib;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Settings.Featured;
public partial class UserDefaultSettingViewModel(BrowserSettingDto bsd, Action OnSelectedChanged, Action<BrowserSettingDto> OnSettingsDeleted) : ViewModelObjectBase {
	[ObservableProperty]
	public string? defaultUrl = bsd.DefaultUrl;
	[ObservableProperty]
	private bool isChecked;

	partial void OnIsCheckedChanged(bool value)
	{
		OnSelectedChanged();
	}

	[RelayCommand]
	public async Task SaveUrlFromViewText()
	{
		if (DefaultUrl.Is() || DefaultUrl == bsd.DefaultUrl) {
			return;
		}

		bsd.DefaultUrl = DefaultUrl;
		_ = await BrowserSettingsRepo.Instance.Put(bsd);
	}

	[RelayCommand]
	public async Task DeleteDefaultSettings()
	{
		_ = await BrowserSettingsRepo.Instance.Delete(bsd.id);
		OnSelectedChanged();
		OnSettingsDeleted(bsd);
	}
}

public partial class UserDefaultSettingsViewModel
			 : ViewModelObjectBase {
	[ObservableProperty]
	private EmulationOptions defaultEmulationOptions = IoC.GetJsonValue<EmulationOptions>(nameof(EmulationOptions)) ?? new();

	private readonly ReadOnlyObservableCollection<UserDefaultSettingViewModel> settings;
	public ReadOnlyObservableCollection<UserDefaultSettingViewModel> ViewModels => settings;

	public bool HasSelectedItems => SelectedCount > 0;
	public int SelectedCount => ViewModels.Count(v => v.IsChecked);

	public UserDefaultSettingsViewModel() : base("Default Browser Settings")
	{
		_ = BrowserSettingsRepo.Instance.ObservableCache
			.Connect()
			.Transform(p=> new UserDefaultSettingViewModel(p, OnSelectedChanged, OnSettingsDeleted))
			.Bind(out settings)
			.Subscribe();
	}

	public override async Task Init(object? param)
	{
		await base.Init(param);

		if (!Loaded) {
			await BrowserSettingsRepo.Instance.Load();
		}
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
		if (_selectedDefaultSetting == null || !_selectedDefaultSetting.Any()) {
			return;
		}

		for (var i = _selectedDefaultSetting.Count() - 1; i >= 0; i--) {
			var setting = _selectedDefaultSetting.ElementAt(i);
			await setting.DeleteDefaultSettings();
		}
		OnSelectedChanged();
	}

	[RelayCommand]
	private async Task CreateNewDefaultSettings()
	{
		_ = await BrowserSettingsRepo.Instance.Create(new BrowserSettingDto() { DefaultUrl = "https://example.com/" });
	}

	[RelayCommand]
	private async Task Save()
	{
		foreach (var viewModel in settings.ToArray()) {
			await viewModel.SaveUrlFromViewText();
		}
		SetDefaultBrowserSettings();
	}

	[RelayCommand]
	private void SaveLaunchSettings()
	{
		IoC.SetJsonValue(DefaultEmulationOptions, nameof(EmulationOptions));
	}

	private void SetDefaultBrowserSettings() {
		IoC.SetJsonValue(settings.Select(v => v.DefaultUrl).ToArray(), "DefaultHomePageSettings");
	}

	private void OnSelectedChanged()
	{
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}

	private void OnSettingsDeleted(BrowserSettingDto _) {
		SetDefaultBrowserSettings();
	}
}