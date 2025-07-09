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
	[ObservableProperty] string defaultUrl = bsd.DefaultUrl ?? "https://example.com/";
	[ObservableProperty] bool isChecked;

	partial void OnIsCheckedChanged(bool value) {
		OnSelectedChanged();
	}

	public async Task SaveUrlFromViewText() {
		if (DefaultUrl.Is() || DefaultUrl == bsd.DefaultUrl) return;

		bsd.DefaultUrl = DefaultUrl;
		_ = await BrowserSettingsRepo.Instance.Put(bsd);
	}

	[RelayCommand]
	public async Task DeleteDefaultSettings() {
		_ = await BrowserSettingsRepo.Instance.Delete(bsd.id);
		OnSelectedChanged();
		OnSettingsDeleted(bsd);
	}
}

public partial class UserDefaultSettingsViewModel
			 : ViewModelObjectBase {
	[ObservableProperty]
	private EmulationOptions defaultEmulationOptions = IoC.GetJsonValue<EmulationOptions>(nameof(EmulationOptions)) ?? new();

	public ReadOnlyObservableCollection<UserDefaultSettingViewModel> ViewModels { get; }

	public bool HasSelectedItems => SelectedCount > 0;
	public int SelectedCount => ViewModels.Count(v => v.IsChecked);

	public UserDefaultSettingsViewModel() : base("Default Browser Settings") {
		void SetDefaultBrowserSettings() {
			var settings = ViewModels?.Where(v => v.IsChecked).Select(v => v.DefaultUrl).ToArray() ?? [];
			IoC.SetJsonValue(settings, "DefaultHomePageSettings");
		}
		_ = BrowserSettingsRepo.Instance.ObservableCache.Connect()
			.Transform(p => new UserDefaultSettingViewModel(p, () => {
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
			}, (_) => SetDefaultBrowserSettings()))
			.Bind(out var settings).Subscribe();
		ViewModels = settings;

		AsyncCommandMap["Add"] = async () => await BrowserSettingsRepo.Instance.Create(new BrowserSettingDto());
		AsyncCommandMap["delete"] = async () => {
			var selected = settings.Where(v => v.IsChecked).ToArray();
			if (selected.Length == 0) return;

			for (var i = selected.Length - 1; i >= 0; i--) {
				await selected[i].DeleteDefaultSettings();
			}
		};

		AsyncCommandMap["Save"] = async () => {
			await settings.ToArray().ForEach(async setting => {
				await setting.SaveUrlFromViewText();
			});
			SetDefaultBrowserSettings();
		};
		CommandMap["UnselectItems"] = () => {
			foreach (var setting in ViewModels) {
				setting.IsChecked = false;
			}
		};
		CommandMap["Save"] = () => {
			IoC.SetJsonValue(DefaultEmulationOptions, nameof(EmulationOptions));
		};
	}

	public override async Task Init(object? param) {
		await base.Init(param);
		if (!Loaded) await BrowserSettingsRepo.Instance.Load();
	}
}