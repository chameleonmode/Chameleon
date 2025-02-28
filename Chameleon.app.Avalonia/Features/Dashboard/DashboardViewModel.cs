using Chameleon.app.Avalonia.Features.Dashboard.Favourite;
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
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.Features.Dashboard;
public partial class DashboardViewModel : ViewModelObjectBase {

	//
	[ObservableProperty]
	private bool isSyncChangesBtnVisible = true;
	[ObservableProperty]
	public bool hasCookiesToSync = false;


	[ObservableProperty]
	private ObservableCollection<TagViewModel> tags = [];

	[ObservableProperty]
	private TagViewModel selectedTag = null!;

	[ObservableProperty]
	private bool isFavouriteSelected = true;

	public FavouriteViewModel FavouriteViewModel => FavouriteViewModel.Instance;

	public DashboardViewModel()
		: base("Dashboard") {

		_ = TagsRepo
			.Connect()
			.Transform(item => new TagViewModel() { Name = item.Name })
			.Subscribe(RefreshTags);

		_ = this.WhenValueChanged(x => x.SelectedTag)
			.Where(selectedTag => selectedTag != null)
			.Subscribe(selectedTag => IsFavouriteSelected = selectedTag!.Name == "Favourites");

		AsyncCommandMap["SyncChanges"] = SyncChanges;
		AsyncCommandMap["SyncCookiesClear"] = SyncCookiesClear;
		AsyncCommandMap["SyncCookiesChrome"] = SyncCookiesChrome;
		AsyncCommandMap["SyncCookiesBrave"] = SyncCookiesBrave;
		AsyncCommandMap["SyncCookiesFirefox"] = SyncCookiesFirefox;
	}

	private void RefreshTags(IChangeSet<TagViewModel, string> changeSet) {
		var items = changeSet.Select(change => change.Current).ToList();

		SelectedTag = new TagViewModel { Name = "Favourites", IsSelected = true };
		items.Insert(0, SelectedTag);

		Tags = new ObservableCollection<TagViewModel>(items);
		foreach (var tag in Tags) {
			_ = tag.TagObservable
				.Skip(1)
				.Subscribe(OnTagSelected);
		}
	}

	private void OnTagSelected(TagViewModel selectedTag) {
		foreach (var tag in Tags)
			tag.IsSelected = tag.Name == selectedTag.Name;
		SelectedTag = selectedTag;
	}

	private async Task SyncCookiesChrome() => await SyncCookies(Enums.SystemBrowserType.Chrome);
	private async Task SyncCookiesBrave() => await SyncCookies(Enums.SystemBrowserType.Brave);
	private async Task SyncCookiesFirefox() => await SyncCookies(Enums.SystemBrowserType.Firefox);

	private async Task SyncChanges() {
		await AppStartup.LoadSink(true);
		await CheckForCookies();
	}

	private async Task CheckForCookies() {
		HasCookiesToSync = false;
		try {
			HasCookiesToSync = await PlaywrightCookiesSyncService.Instance.HasCookies();
		} catch (Exception e) {
			Toaster.Error("Failed to check for cookies. " + e.Message);
		}
	}

	private async Task SyncCookies(Enums.SystemBrowserType systemBrowserType) {
		try {
			await PlaywrightCookiesSyncService.Instance.SyncCookies(systemBrowserType);
			Toaster.Success("Cookies Synced");
		} catch (Exception e) {
			Toaster.Error("Failed to sync cookies. " + e.Message);
		}
	}
	private async Task SyncCookiesClear() {
		try {
			await DB.Instance.DeleteDataInteractions();
			Toaster.Success("Cookies Cleared");
		} catch (Exception e) {
			Toaster.Error("Failed to sync cookies. " + e.Message);
		}
		await CheckForCookies();
	}
}

public partial class TagViewModel : ObservableObject {

	public BehaviorSubject<TagViewModel> TagObservable { get; } = null!;

	[ObservableProperty]
	private string name = null!;

	[ObservableProperty]
	private bool isSelected;

	public TagViewModel() {

		TagObservable = new(this);

		_ = this.WhenValueChanged(x => x.IsSelected)
								.Where(isSelected => isSelected)
								.Subscribe(_ => TagObservable.OnNext(this));
	}
}
