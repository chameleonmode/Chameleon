using Chameleon.client.Features.Shared.Tags;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Dashboard;
public partial class DashboardViewModel : ViewModelObjectBase {
	[ObservableProperty] bool isSyncChangesBtnVisible = true;
	[ObservableProperty] bool hasCookiesToSync = false;
	[ObservableProperty] bool isFavouriteSelected = true;
	[ObservableProperty] TagViewModel? selectedTag;

	public ReadOnlyObservableCollection<TagViewModel> Tagz { get; }

	public DashboardViewModel()
		: base("Dashboard") {

		_ = TagsRepo.Connect()
			.Filter(tag => tag.Name == "Favourites" || tag.Items.Any(x => x.Value.Count > 0))
			.Transform(item => new TagViewModel(t => SelectedTag = t) { Name = item.Name })
			.Bind(out var tagz)
			.Subscribe(changeSet => {
				// if (Tagz.Count > 0) {
				// 	foreach (var change in changeSet) {
				// 		if (!Tagz.Any(tag => tag.Name == change.Current.Name)) continue;
				// 		else if(change.Reason == ChangeReason.Remove) _ = Tagz.Remove(change.Current);
				// 		else Tagz.Add(change.Current);
				// 	}
				// } else {
				// 	Tagz.Add(new TagViewModel { Name = "Favourites", IsSelected = true });
				// 	Tagz.AddRange(changeSet.Select(change => change.Current));
				// 	SelectedTag = Tagz[0];
				// }

				// foreach (var tag in Tagz) {
				// 	_ = tag.TagObservable
				// 		.Skip(1)
				// 		.Subscribe(OnTagSelected);
				// }

				SelectedTag ??= tagz[0];
			});
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

		if(!newValue.IsSelected) newValue.IsSelected = true;
		if (oldValue != null && oldValue.IsSelected) {
			 oldValue.IsSelected = false;
		}
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
