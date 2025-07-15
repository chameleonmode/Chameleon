using Chameleon.client.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Chameleon.lib.Util;
using Chameleon.lib;
using Chameleon.lib.Browzer;

namespace Chameleon.client.Features.Settings.Featured;

public partial class UserDefaultSettingViewModel(string url, Action<UserDefaultSettingViewModel> OnSettingsDeleted) : OOVM {
	[ObservableProperty] string defaultUrl = url;
	[ObservableProperty] bool isChecked;

	[RelayCommand]
	public void DeleteDefaultSettings() {
		OnSettingsDeleted(this);
	}
}

public partial class UserDefaultSettingsViewModel : OOVM {
	[ObservableProperty] EmulationOptions defaultEmulationOptions = IoC.GetJsonValue<EmulationOptions>(nameof(EmulationOptions)) ?? new();

	public ObservableCollection<UserDefaultSettingViewModel> ViewModels { get; } = [];

	public bool HasSelectedItems => SelectedCount > 0;
	public int SelectedCount => ViewModels.Count(v => v.IsChecked);

	public UserDefaultSettingsViewModel() : base("Default Browser Settings") {
		var bookmarks = IoC.GetJsonValue<string[]>("Bookmarks")
		 .Let(urls => urls != null && urls.Length > 0 ? new[] { urls[new Random().Next(urls.Length)] } : ["example.com"]);
		bookmarks.ForEach(b => ViewModels.Add(new(b, (vm) => ViewModels.Remove(vm))));

		AsyncCommandMap["Add"] = () => { ViewModels.Add(new("example.com", (vm) => ViewModels.Remove(vm))); return Task.CompletedTask; };
		AsyncCommandMap["delete"] = async () => {
			await Task.CompletedTask;
			var selected = ViewModels.Where(v => v.IsChecked).ToArray();
			for (var i = selected.Length - 1; i >= 0; i--) {
				 selected[i].DeleteDefaultSettings();
			}
		};

		AsyncCommandMap["Save"] = async () => {
			await Task.CompletedTask;
			var settings = ViewModels?.Where(v => v.IsChecked).Select(v => v.DefaultUrl).ToArray() ?? [];
			IoC.SetJsonValue(settings, "Bookmarks");
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
}