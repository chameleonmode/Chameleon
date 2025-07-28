using Chameleon.client.Features.Dashboard.Tags;
using Chameleon.client.Features.Dashboard.Favorite;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Chameleon.client.Features.Projects;
using Chameleon.lib.Browzio;
using Chameleon.lib.Util;
using Chameleon.client.Features.Projects.Folders;

namespace Chameleon.client.Features.Dashboard;

public abstract partial class Dashboarder(string? title) : Profilearee(title) {
	[ObservableProperty] ChangeComparereOption sortFolder = ChangeComparereOption.Ascending;

	public abstract ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	public bool HasFolders => Folders.Count > 0;
	public bool HasNoItems => !HasFolders && !HasProfiles;

	partial void OnSortFolderChanged(ChangeComparereOption value) {
		Folderer.CompareObservable.OnNext(value switch {
			ChangeComparereOption.Descending => Folderer.DescendingComparer,
			_ => Folderer.AscendingComparer
		});
	}
}

public partial class TagViewModel(Action<TagViewModel> OnSelectChanged) : ObservableObject {
	[ObservableProperty] string name = null!;
	[ObservableProperty] bool isSelected;

	partial void OnIsSelectedChanged(bool value) {
		if (value) OnSelectChanged(this);
	}
}

public partial class ViewModel : OOVM {
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
			await Modules.Sync();
			await CheckForCookies();
		};
		AsyncCommandMap["SyncCookiesClear"] = async () => {
			await DB.I.Cooky.Delete();
			await CheckForCookies();
		};
		AsyncCommandMap["SyncCookiesChrome"] = async () => await SyncCookies(BrowserType.Chrome);;
		AsyncCommandMap["SyncCookiesBrave"] = async () => await SyncCookies(BrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = async () =>  await SyncCookies(BrowserType.Firefox);
	}

	public override async Task Init(object? param) {
		await base.Init(param);
		ProfileUIContextManager.ApplyContextToProfiles(FavouriteViewModel.Instance.Profiles, ProfileUIContext.Dashboard);
		ProfileUIContextManager.ApplyContextToProfiles(TagsViewModel.Instance.Profiles, ProfileUIContext.Dashboard);
	} 

	partial void OnSelectedTagChanged(TagViewModel? oldValue, TagViewModel? newValue) {
		if (newValue == null) return;

		IsFavouriteSelected = newValue.Name == "Favourites";

		// Apply Favorites context when Favourites tab is selected
		if (IsFavouriteSelected) ProfileUIContextManager.SetModuleContext(ProfileUIModule.Favourites, ProfileUIContext.Dashboard);
		else TagsViewModel.Instance.SelectedTagName = newValue.Name;

		if (!newValue.IsSelected) newValue.IsSelected = true;
		if (oldValue != null && oldValue.IsSelected) oldValue.IsSelected = false;
	}

	private async Task CheckForCookies() {
		HasCookiesToSync = false;
		try {
			HasCookiesToSync = await Sync.Instance.HasCookies();
		} catch (Exception e) {
			Toaster.Error("Failed to check for cookies. " + e.Message);
		}
	}

	static async Task SyncCookies(BrowserType systemBrowserType) {
		await Sync.Instance.SyncCookies(systemBrowserType);
		Toaster.Success("Cookies Synced");
	}
	public static ViewModel Instance { get; } = new();
}
