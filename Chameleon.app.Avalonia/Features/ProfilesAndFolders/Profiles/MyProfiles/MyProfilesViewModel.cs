using Avalonia.Collections;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Extensions;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Folders;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModels;
using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Models;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Playwright.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
public partial class MyProfilesViewModel : ViewModelObjectBase {
	private readonly TagsRepo tagsRepo = TagsRepo.Instance;

	public AvaloniaList<RunScriptOptions> PlaywrightScripts { get; } = [];
	[ObservableProperty]
	private RunScriptOptions? selectedPlaywrightScript;

	[ObservableProperty]
	private PaginatorViewModel paginatorViewModel;
	[ObservableProperty]
	private SystemBrovserItem? selectedBrowserItem;
	[ObservableProperty]
	private int totalCount;
	[ObservableProperty]
	private bool hasFolder;
	[ObservableProperty]
	private bool isVisibleRunButton = true;
	[ObservableProperty]
	private bool isVisibleStopButton;
	[ObservableProperty]
	private bool isVisibleWaitButton;
	[ObservableProperty]
	private bool isRecordSelected;
	[ObservableProperty]
	private string searchText = string.Empty;
	[ObservableProperty]
	private UPFolderViewModel? folder;

	private CancellationTokenSource? _cts;

	public ObservableCollection<SystemBrovserItem> BrowserItems { get; } = [
		new SystemBrovserItem(SystemBrowserType.Brave),
		new SystemBrovserItem(SystemBrowserType.Chrome)
	];
	private CancellationToken RecreateCancellationToken {
		get {
			if (_cts != null) {
				_cts.Cancel();
				_cts.Dispose();
			}

			_cts = new CancellationTokenSource();
			return _cts.Token;
		}
	}

	private IEnumerable<ObsProfile> GetSelectedProfiles => Profiles.Where(i => i.IsSelected);
	public int SelectedCount => GetSelectedProfiles?.Count() ?? 0;
	public int MaxInFolderItems => Folder == null || Folder!.Id == 0
	? UserProfilesRepo.Instance.ObservableCache.Count
	: UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == Folder.Id);
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public bool IsProfilesExist => FoldersViewModel.Instance.AllProfiles?.IsFolderNotEmpty == false;
	public bool HasNoItems => Profiles.Count == 0;
	public bool HasProfileWithoutFolder => Profiles != null && Profiles.Any(profile => profile.Dto?.folderId != null);
	public string SelectedFolderTitle => Folder?.Title ?? "All profiles";
	//
	public Func<ObsProfile, bool> FilterPredicate => p => Folder == null || Folder.Id == 0 || (Folder != null && Folder.Id != 0 && p.Dto?.folderId == Folder?.Id);

	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.ObsProfileCompares.AscendingComparer);
	private readonly BehaviorSubject<IPageRequest> pageRequests = new(new PageRequest(0, Consts.PageinationPageItems));
	private readonly BehaviorSubject<Func<ObsProfile, bool>> filter;

	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	private readonly ReadOnlyObservableCollection<ObsProfile> profiles;
	public ReadOnlyObservableCollection<ObsProfile> Profiles => profiles;

	public MyProfilesViewModel() {
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(FilterPredicate);
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i, onSelectedChanged: p => {
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
			}))
			.Filter(filter)
			.SortAndPage(Compares.ObsProfileCompares.AscendingComparer, pageRequests)
			.SortAndBind(out profiles, profilesCompareObservable)
			.Subscribe((i) => {
			});

		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) 
		{
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};

		TotalCount = PaginatorViewModel.TotalCount;
		AsyncCommandMap["SaveTags"] = async () => {
			_ = await tagsRepo.SaveTagsAsync(TagItemType.Folder, Folder!.Id.ToString(), Folder.Tags.ToTagsList());
		};
	}
	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);

		PlaywrightScripts.Clear();
		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		var usd = IoC.GetValue<string>("UserScriptsDirectory");
		if (usd.Is() && Directory.Exists(usd)) {
			PlaywrightScripts.AddRange(await BundledScriptsService.GetUserScripts(usd));
		}

		//InintializeLastSelectedAutomation();
		var lastSelectedBrowserString = IoC.GetValue<string>("LastSelectedBrowser");
		SelectedBrowserItem = !lastSelectedBrowserString.Is() ||
				!Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum)
			? BrowserItems[0]
			: BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);
		SelectedPlaywrightScript = PlaywrightScripts.FirstOrDefault(s => s.Description?.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];

		//SetViewModelsFilter();
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value) {
		profilesCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsProfileCompares.DescendingComparer,
			_ => Compares.ObsProfileCompares.AscendingComparer
		});
	}
	partial void OnSelectedBrowserItemChanged(SystemBrovserItem? value) {
		if (value == null)
			return;

		var cur = IoC.GetValue<string>("LastSelectedBrowser");
		if (cur != value.SystemBrowserType.ToString())
			IoC.SetValue(value.SystemBrowserType.ToString(), "LastSelectedBrowser");
	}
	partial void OnSelectedPlaywrightScriptChanged(RunScriptOptions? value) {
		var cur = IoC.GetValue<string>("LastRunScriptId");
		if (value != null && cur != value.Description?.Title)
			IoC.SetValue(value.Description?.Title, "LastRunScriptId");
	}
	partial void OnSearchTextChanged(string value) {
		if (value.Is()) {
			PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
			filter.OnNext(p => p.Title?.Contains(value, StringComparison.CurrentCultureIgnoreCase) == true && (Folder == null || Folder.id == 0 || (Folder != null && Folder.id != 0 && p.Dto?.folderId == Folder?.id)));
		} else {
			PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
			filter.OnNext(FilterPredicate);
		}

		SetViewModelsFilter(false);
	}
	partial void OnFolderChanged(UPFolderViewModel? value) {

		FoldersViewModel.Instance.SetSelectedFolder(value?.ToDto());
		SearchText = string.Empty;
		HasFolder = value?.id != default && value?.id != 0;
		OnPropertyChanged(nameof(SelectedFolderTitle));
		SetViewModelsFilter();
	}

	public async Task OpenAsync(UPFolderDto? folder) {
		if (folder is not null) {
			Folder = new UPFolderViewModel(folder!);
			Folder.Tags = await tagsRepo.GetTagsAsync(TagItemType.Folder, Folder.Id.ToString()).ToStringAsync();
			OnPropertyChanged(nameof(SelectedFolderTitle));
			UnselectItems();
			SetViewModelsFilter();
		}
	}
	public async Task<UserProfileDto?> CreateNewProfile() {
		var folderId = HasFolder ? Folder?.Id : null;

		var pcount = UserProfilesRepo.Instance.ObservableCache.Items.Count;
		var pname = $"New Profile - {pcount}";
		while (UserProfilesRepo.Instance.ObservableCache.Items.Any(i => i.title == pname))
			pname = $"New Profile - {++pcount}";

		var res = await UserProfilesRepo.CreateProfile(pname, folderId);
		if (res != null) {
			SetViewModelsFilter();
		}

		return res;
	}
	public async void OnFilterTo(ObsProfile? p = null) {
		_ = await LoadedTCS.Task;
		await Task.Delay(100);

		if (p != null) {
			if (p.Dto?.folderId is int fid && fid != 0)
				FoldersViewModel.Instance.SetSelectedById(fid);
			else
				await FoldersViewModel.Instance.OnNavigatingTo(null);

			SearchText = p.Title ?? string.Empty;
		} else {
			FoldersViewModel.Instance.SetSelectedById(0);
			SearchText = string.Empty;
		}

		OnPropertyChanged(nameof(SearchText));
	}
	public void SetViewModelsFilter(bool onext = true) {
		if (onext)
			filter.OnNext(FilterPredicate);

		TotalCount = PaginatorViewModel.TotalCount = MaxInFolderItems;

		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(HasProfileWithoutFolder));
	}

	[RelayCommand]
	private void SelectAll() {
		foreach (var profile in Profiles) {
			profile.IsSelected = true;
		}
	}

	[RelayCommand]
	private void SelectAllProfilesFromFolder() {
		PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
		SelectAll();
	}

	[RelayCommand]
	private void UnselectItems() {
		foreach (var profile in Profiles) {
			profile.IsSelected = false;
		}
		PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
	}

	[RelayCommand]
	private async Task Delete() {
		if (GetSelectedProfiles == null) {
			return;
		}

		if (await Mbox.Show("Delete User Profiles",
				$"Are you sure you want to delete {SelectedCount} profiles?",
				MBoxButtons.OkCancel,
				"DeleteLines")) {
			var profiles = GetSelectedProfiles.ToList();

			foreach (var profile in profiles) {
				var res = await UserProfilesRepo.Instance.Delete(profile.Dto!.id);
				if (!res.success) {
					profile.IsSelected = false;
				}
			}
			SetViewModelsFilter();
		}
	}

	[RelayCommand]
	private async Task RemoveProfilesFromFolder() {
		if (Folder?.Id == 0 ||
				GetSelectedProfiles == null ||
				!GetSelectedProfiles.Any()) {
			return;
		}

		var ids = GetSelectedProfiles
				.Select(a => a.Dto!.id)
				.ToList();

		var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, null);
		if (!res.success) {
		}

		SetViewModelsFilter();
	}

	[RelayCommand]
	private async Task AddProfilesToFolder() {
		if (Folder == null || Folder.Id == 0)
			return;

		var addvm = new AddUserProfilesPupViewModel {
			Title = "Add Profiles"
		};

		if (await Mbox.ShowTaskDialog<AddUserProfilesPupViewModel, AddUserProfilesPopupUserControl>(() => addvm,
			header: addvm.Title,
			subHeader: $"Select profiles you want to add to {Folder!.Title} folder:",
			symbas: Enums.Symbas.Folder,
			btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
			var ids = addvm.SelectedProfiles?
				.Select(a => a.Dto!.id)
				.ToList();
			if (ids == null || ids.Count == 0) {
				return;
			}
			var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, Folder!.Id);
			if (!res.success) {
			}
		}

		SetViewModelsFilter();
	}

	[RelayCommand]
	private async Task MoveProfilesToFolder() {
		var selectedProfiles = Profiles
			.Where(p => p.IsSelected);
		if (!selectedProfiles.Any()) {
			return;
		}

		var addvm = new MoveUserProfilesPopupViewModel {
			Title = "Add To Folder"
		};
		addvm.Profiles.AddRange(selectedProfiles);
		addvm.Folders.AddRange(FoldersViewModel.Instance.Folders);

		if (await Mbox.ShowTaskDialog<MoveUserProfilesPopupViewModel, MoveUserProfilesPopupUserControl>(() => addvm,
			header: addvm.Title,
			subHeader: $"Select a folder to move the {selectedProfiles.Count()} selected profiles:",
			symbas: Enums.Symbas.Folder,
			btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
			if (addvm.SelectedFolder is null || !addvm.Profiles.Any()) {
				return;
			}
			var ids = addvm.Profiles
				.Select(a => a.Dto!.id)
				.ToList();

			await UserProfilesRepo.MoveUserProfileToFolder(ids, addvm.SelectedFolder.Dto!.id);
		}

		SetViewModelsFilter();
	}

	[RelayCommand]
	private void OpenChameleonBrowser() {
		GetSelectedProfiles?.ForEach(profile => {
			profile.OpenUserBrowser();
		});
	}

	[RelayCommand]
	private void OpenFirefox() {
		OpenSystemBrowser(SystemBrowserType.Firefox);
	}

	[RelayCommand]
	private void OpenChrome() {
		OpenSystemBrowser(SystemBrowserType.Chrome);
	}

	[RelayCommand]
	private void OpenBrave() {
		OpenSystemBrowser(SystemBrowserType.Brave);
	}

	private void OpenSystemBrowser(SystemBrowserType browserType) {
		GetSelectedProfiles?.ForEach(async (selectedProfile) => {
			await selectedProfile.OpenSystemBrowser(browserType);
		});
	}

	[RelayCommand]
	private async Task RunAutomation() {
		if (!GetSelectedProfiles.Any()) {
			Toaster.Error("Select one or more profiles to run the automation.");
			return;
		}

		IsVisibleRunButton = false;
		IsVisibleStopButton = true;

		try {
			var token = RecreateCancellationToken;
			foreach (var profile in GetSelectedProfiles) {
				var browserWasNotOpened = profile.SBI![SelectedBrowserItem!.SystemBrowserType] == null;
				if (browserWasNotOpened) {
					await profile.OpenSystemBrowser(SelectedBrowserItem.SystemBrowserType).WaitAsync(token);
					if (profile.SBI![SelectedBrowserItem.SystemBrowserType] == null || !await profile.SBI![SelectedBrowserItem.SystemBrowserType]!.LoadedTCS.Task.WaitAsync(token))
						continue;
				}
				SelectedPlaywrightScript!.Port = profile.SBI![SelectedBrowserItem.SystemBrowserType]!.Settings.Port;
				SelectedPlaywrightScript.Record = IsRecordSelected;
				try {
					await PlaywriteRunner.RunScript(SelectedPlaywrightScript, token);
				} catch (Exception ex) {
					// Log or handle the exception if closing the process fails
					Toaster.Error($"{ex.Message}");
				}

				// Check if the browser process is not null and hasn't exited
				if (browserWasNotOpened) {
					await ProUtil.TryKillProcess(profile.SBI[SelectedBrowserItem.SystemBrowserType]?.Brocess);
				}

				// Stop loop if canceled
				if (token.IsCancellationRequested) {
					break;
				}
			}
		} catch (Exception ex) {
			Toaster.Error($"{ex.Message}");
		} finally{
			IsVisibleRunButton = true;
			IsVisibleStopButton = false;
			IsVisibleWaitButton = false;
		}
	}

	[RelayCommand]
	private void StopAutomation() {
		IsVisibleStopButton = false;
		IsVisibleWaitButton = true;
		_cts?.Cancel();
	}

	public static MyProfilesViewModel Instance { get; } = new MyProfilesViewModel();
}
