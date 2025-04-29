using Chameleon.client.Features.Dashboard.Tags;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Dashboard;

public partial class TagViewModel(Action<TagViewModel> OnSelectChanged) : ObservableObject {
	[ObservableProperty] string name = null!;
	[ObservableProperty] bool isSelected;

	partial void OnIsSelectedChanged(bool value) {
		if (value) OnSelectChanged(this);
	}
}

public partial class ViewModel : ViewModelObjectBase {
	[ObservableProperty] bool isSyncChangesBtnVisible = true;
	[ObservableProperty] bool hasCookiesToSync = false;
	[ObservableProperty] bool isFavouriteSelected = true;
	[ObservableProperty] TagViewModel? selectedTag;

	public ReadOnlyObservableCollection<TagViewModel> Tagz { get; }

	public ViewModel() : base("Dashboard") {
		_ = TagsRepo.Connect()
			.Filter(tag => tag.Name == "Favourites" || tag.Items.Any(x => x.Value.Count > 0))
			.Transform(item => new TagViewModel(t => SelectedTag = t) { Name = item.Name, IsSelected = item.Name == "Favourites" })
			.Bind(out var tagz)
			.Subscribe();
		Tagz = tagz;

		AsyncCommandMap["SyncChanges"] = async () => {
			await app.Avalonia.AppStartup.LoadSink(true);
			await CheckForCookies();
		};
		AsyncCommandMap["SyncCookiesClear"] = async () => {
			await DB.Instance.DeleteDataInteractions(DB.Routes.Cooky.DataType);
			Toaster.Success("Cookies Cleared");

			await CheckForCookies();
		};
		AsyncCommandMap["SyncCookiesChrome"] = async () => await SyncCookies(Enums.SystemBrowserType.Chrome);;
		AsyncCommandMap["SyncCookiesBrave"] = async () => await SyncCookies(Enums.SystemBrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = async () =>  await SyncCookies(Enums.SystemBrowserType.Firefox);
	}

	partial void OnSelectedTagChanged(TagViewModel? oldValue, TagViewModel? newValue) {
		if (newValue == null) return;

		IsFavouriteSelected = newValue.Name == "Favourites";
		
		if (!IsFavouriteSelected) TagsViewModel.Instance.SelectedTagName = newValue.Name;
		
		if (!newValue.IsSelected) newValue.IsSelected = true;
		if (oldValue != null && oldValue.IsSelected) oldValue.IsSelected = false;
	}

	private async Task CheckForCookies() {
		HasCookiesToSync = false;
		try {
			HasCookiesToSync = await PlaywrightCookiesSyncService.Instance.HasCookies();
		} catch (Exception e) {
			Toaster.Error("Failed to check for cookies. " + e.Message);
		}
	}

	static async Task SyncCookies(Enums.SystemBrowserType systemBrowserType) {
		await PlaywrightCookiesSyncService.Instance.SyncCookies(systemBrowserType);
		Toaster.Success("Cookies Synced");
	}
}
