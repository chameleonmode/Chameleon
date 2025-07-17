using Chameleon.client.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Chameleon.lib.Util;
using Chameleon.lib;
using Chameleon.lib.Browzio;

namespace Chameleon.client.Features.Settings.Featured;

public partial class UserDefaultSettingViewModel(string url, UserDefaultSettingsViewModel parent) : OOVM {
	[ObservableProperty] string defaultUrl = url;
	[ObservableProperty] bool isChecked;

	[RelayCommand]
	public void Delete() {
		parent.ViewModels.Remove(this);
		parent.RefreshProperties();
	}

	partial void OnIsCheckedChanged(bool value) => parent.RefreshProperties();
}

public partial class UserDefaultSettingsViewModel : OOVM {
	[ObservableProperty] EmulationOptions defaultEmulationOptions = IoC.GetJsonValue<EmulationOptions>(nameof(EmulationOptions)) ?? new();
	[ObservableProperty] string? startPage = IoC.GetValue(nameof(StartPage));

	public ObservableCollection<UserDefaultSettingViewModel> ViewModels { get; } = [];

	public bool HasSelectedItems => SelectedCount > 0;
	public int SelectedCount => ViewModels.Count(v => v.IsChecked);

	public UserDefaultSettingsViewModel() : base("Default Browser Settings") {
		var bookmarks = IoC.GetJsonValue<string[]>("Bookmarks") ?? [];
		bookmarks.ForEach(b => ViewModels.Add(new(b, this)));

		AsyncCommandMap["Add"] = () => {
			ViewModels.Add(new("", this)); return Task.CompletedTask;
		};
		AsyncCommandMap["delete"] = async () => {
			await Task.CompletedTask;
			var selected = ViewModels.Where(v => v.IsChecked).ToArray();
			for (var i = selected.Length - 1; i >= 0; i--) {
				selected[i].Delete();
			}
		};

		AsyncCommandMap["Save"] = async () => {
			await Task.CompletedTask;
			var settings = ViewModels.Select(v => v.DefaultUrl).ToArray() ?? [];
			IoC.SetJsonValue(settings, "Bookmarks");
			if (StartPage.IsNot()) IoC.SetValue(nameof(StartPage), StartPage);
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

	public void RefreshProperties() {
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
}