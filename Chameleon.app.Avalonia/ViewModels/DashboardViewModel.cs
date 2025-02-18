using CommunityToolkit.Mvvm.ComponentModel;

using DynamicData;

using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.ObjectModel;

using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Helpers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Abs.Platformatic;

using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Playwright.Services;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class DashboardViewModel : ViewModelObjectBase {
	// Private fields
	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.ObsProfileCompares.AscendingComparer);
	private readonly BehaviorSubject<IComparer<ObsFolder>> foldersCompareObservable = new(Compares.ObsFolderCompares.AscendingComparer);

	//
	[ObservableProperty]
	private bool isSyncChangesBtnVisible = true;
	[ObservableProperty]
	public bool hasCookiesToSync = false;
	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;
	[ObservableProperty]
	private Enums.ChangeComparereOption folderSortSelected = Enums.ChangeComparereOption.Ascending;

	//
	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	//
	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;

	public DashboardViewModel() 
		: base("Dashboard")
	{
		//
		_ = UserProfilesRepo
			.Connect(i => i.isFavourite)
			.Transform(i => new ObsProfile(i, false))
			.SortAndBind(out var list, profilesCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoItems)); 
			});
		Profiles = list;

		//
		_ = UserProfilesFolderRepo
			.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i, true, null))
			.SortAndBind(out var flist, foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;

		AsyncCommandMap["SyncChanges"] = SyncChanges;
		AsyncCommandMap["SyncCookiesChrome"] = SyncCookiesChrome;
		AsyncCommandMap["SyncCookiesBrave"] = SyncCookiesBrave;
		AsyncCommandMap["SyncCookiesFirefox"] = SyncCookiesFirefox;
		AsyncCommandMap["SyncCookiesClear"] = SyncCookiesClear;
	}

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		//if(!Loaded) {
		//	await SyncChanges();
		//}
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		profilesCompareObservable.OnNext(value switch { 
			Enums.ChangeComparereOption.Descending => Compares.ObsProfileCompares.DescendingComparer,
			_ => Compares.ObsProfileCompares.AscendingComparer
		});
	}

  partial void OnFolderSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		foldersCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsFolderCompares.DescendingComparer,
			_ => Compares.ObsFolderCompares.AscendingComparer
		});
	}

	private async Task SyncChanges()
	{
		await AppStartup.LoadSink(true);
		await CheckForCookies();
	}

	private async Task CheckForCookies() {
		HasCookiesToSync = false;
		try {
			HasCookiesToSync = await PlaywrightCookiesSyncService.Instance.HasCookies();
		} catch (Exception e) {
			Toaster.Error("Failed to check for cookies. Please try again. " + e.Message);
		}
	}

	private async Task SyncCookies(Enums.SystemBrowserType systemBrowserType)
	{
		try {
			await PlaywrightCookiesSyncService.Instance.SyncCookies(systemBrowserType);
			Toaster.Success("Cookies Synced");
		} catch (Exception e) {
			Toaster.Error("Failed to sync cookies. Please try again. " + e.Message);
		}
	}
	private async Task SyncCookiesChrome() => await SyncCookies(Enums.SystemBrowserType.Chrome);
	private async Task SyncCookiesBrave() => await SyncCookies(Enums.SystemBrowserType.Brave);
	private async Task SyncCookiesFirefox() => await SyncCookies(Enums.SystemBrowserType.Firefox);


	private async Task SyncCookiesClear()
	{
		try {
			await PlatformaticDB.Instance.DeleteDataInteractions();
			Toaster.Success("Cookies Cleared");
		} catch (Exception e) {
			Toaster.Error("Failed to sync cookies. Please try again. " + e.Message);
		}
		await CheckForCookies();
	}
}

