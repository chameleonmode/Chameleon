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
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright;
using Chameleon.lib.Playwright.Services;
using Chameleon.lib.Util;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
public partial class MyProfilesViewModel : ViewModelObjectBase {
	private readonly TagsRepo tagsRepo = TagsRepo.Instance;
	private CancellationTokenSource? cts;

	[ObservableProperty] RunScriptOptions selectedPlaywrightScript;
	[ObservableProperty] PaginatorViewModel paginatorViewModel;
	[ObservableProperty] SystemBrovserItem selectedBrowserItem;
	[ObservableProperty] int totalCount;
	[ObservableProperty] bool hasFolder;
	[ObservableProperty] bool isVisibleRunButton = true;
	[ObservableProperty] bool isVisibleStopButton;
	[ObservableProperty] bool isVisibleWaitButton;
	[ObservableProperty] bool isRecordSelected;
	[ObservableProperty] string searchText = string.Empty;
	[ObservableProperty] UPFolderViewModel? folder;

	public AvaloniaList<RunScriptOptions> PlaywrightScripts { get; } = [];
	public ObservableCollection<SystemBrovserItem> BrowserItems { get; } = [
		new SystemBrovserItem(SystemBrowserType.Chrome),
		new SystemBrovserItem(SystemBrowserType.Brave),
	];

	private IEnumerable<ObsProfile> GetSelectedProfiles => Profiles.Where(i => i.IsSelected);
	public int SelectedCount => GetSelectedProfiles?.Count() ?? 0;
	public int MaxInFolderItems => Folder == null || Folder!.Id == 0
	? UserProfilesRepo.Instance.ObservableCache.Count
	: UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == Folder.Id);
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public bool IsProfilesExist => UserProfilesRepo.Instance.ObservableCache.Items.Any();
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
	public event Action<ObsProfile>? OnSelectedChanged;

	public MyProfilesViewModel() {
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(FilterPredicate);
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i, onSelectedChanged: p => {
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
				OnSelectedChanged?.Invoke(p);
			}))
			.Filter(filter)
			.SortAndPage(Compares.ObsProfileCompares.AscendingComparer, pageRequests)
			.SortAndBind(out profiles, profilesCompareObservable)
			.Subscribe();

		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		TotalCount = PaginatorViewModel.TotalCount;

		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		SelectedPlaywrightScript =
			PlaywrightScripts.FirstOrDefault(s => s.Description?.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];

		SelectedBrowserItem = BrowserItems[0];

		CommandMap["SelectAll"] = () => {
			SelectAll();
		};
		CommandMap["SelectAllProfilesFromFolder"] = () => {
			PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
			SelectAll();
		};
		CommandMap["UnselectItems"] = () => {
			foreach (var profile in Profiles) {
				profile.IsSelected = false;
			}
			PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
		};

		AsyncCommandMap["SaveTags"] = async () => {
			_ = await tagsRepo.SaveTagsAsync(TagItemType.Folder, Folder!.Id.ToString(), Folder.Tags.ToTagsList());
		};
		AsyncCommandMap["chrome"] = async () => {
			await OpenSystemBrowser(SystemBrowserType.Chrome);
		};
		AsyncCommandMap["brave"] = async () => {
			await OpenSystemBrowser(SystemBrowserType.Brave);
		};
		AsyncCommandMap["firefox"] = async () => {
			await OpenSystemBrowser(SystemBrowserType.Firefox);
		};
		AsyncCommandMap["hwinds"] = () => {
			GetSelectedProfiles?.ForEach(profile => {
				profile.OpenTopmostController();
			});
			return Task.CompletedTask;
		};
		AsyncCommandMap["play"] = async () => {
			await RunAutomation();
		};
		AsyncCommandMap["Move"] = async () => {
			if (!GetSelectedProfiles.Any()) return;

			var addvm = new MoveUserProfilesPopupViewModel {
				Title = "Add To Folder"
			};
			addvm.Profiles.AddRange(GetSelectedProfiles);
			addvm.Folders.AddRange(FoldersViewModel.Instance.Folders);

			if (
				await Mbox.ShowTaskDialog<MoveUserProfilesPopupUserControl, MoveUserProfilesPopupViewModel>(new(
					Initialize: () => addvm,
					Header: addvm.Title,
					SubHeader: $"Select a folder to move the {GetSelectedProfiles.Count()} selected profiles:",
					Symbas: Enums.Symbas.Folder,
					Btns: Enums.MBoxButtons.OkCancel)) == Enums.TaskDialogResult.OK &&
					addvm.SelectedFolder is not null && addvm.Profiles.Any()
			) {
				_ = await UserProfilesRepo.MoveUserProfileToFolder(addvm.Profiles.Select(a => a.Dto!.id), addvm.SelectedFolder.Dto!.id);
			}

			SetViewModelsFilter();
		};
		AsyncCommandMap["Remove"] = async () => {
			if (!GetSelectedProfiles.Any()) return;

			_ = await UserProfilesRepo.MoveUserProfileToFolder(GetSelectedProfiles.Select(a => a.Dto!.id), null);
			SetViewModelsFilter();
		};
		AsyncCommandMap["Delete"] = async () => {
			if (
				GetSelectedProfiles.Any() &&
				await Mbox.Show(
					"Delete User Profiles",
					$"Are you sure you want to delete {SelectedCount} profiles?",
					MBoxButtons.OkCancel,
					"DeleteLines")
			) {
				var profiles = GetSelectedProfiles.ToList();

				foreach (var profile in profiles) {
					var res = await UserProfilesRepo.Instance.Delete(profile.Dto!.id);
					if (!res.success) {
						profile.IsSelected = false;
					}
				}
				SetViewModelsFilter();
			}
		};
		AsyncCommandMap["AddProfilesToFolder"] = async () => {
			if (Folder == null || Folder.Id == 0) return;

			var addvm = new AddUserProfilesPupViewModel {
				Title = "Add Profiles"
			};

			if (
				await Mbox.ShowTaskDialog<AddUserProfilesPopupUserControl, AddUserProfilesPupViewModel>(new(
					Initialize: () => addvm,
					Header: addvm.Title,
					SubHeader: $"Select profiles you want to add to {Folder!.Title} folder:",
					Symbas: Enums.Symbas.Folder,
					Btns: Enums.MBoxButtons.OkCancel)) == Enums.TaskDialogResult.OK
			) {
				if (!addvm.SelectedProfiles.Any()) return;
				_ = await UserProfilesRepo.MoveUserProfileToFolder(addvm.SelectedProfiles.Select(o => o.Dto.id), Folder!.Id);
			}

			SetViewModelsFilter();
		};
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
		Profiles.ForEach(p => p.IsActionOptionsVisible = true);

		PlaywrightScripts.Clear();
		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		var usd = IoC.GetValue<string>("UserScriptsDirectory");
		if (usd.IsNot() && Directory.Exists(usd)) {
			PlaywrightScripts.AddRange(await BundledScriptsService.GetUserScripts(usd));
		}

		//InintializeLastSelectedAutomation();
		SelectedBrowserItem =
			Enum.TryParse<SystemBrowserType>(IoC.GetValue<string>("LastSelectedBrowser"), out var browserEnum)
			? BrowserItems.FirstOrDefault(b => b.SystemBrowserType == browserEnum) ?? BrowserItems[0] : BrowserItems[0];
		SelectedPlaywrightScript =
			PlaywrightScripts.FirstOrDefault(s => s.Description?.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value) {
		profilesCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsProfileCompares.DescendingComparer,
			_ => Compares.ObsProfileCompares.AscendingComparer
		});
	}
	partial void OnSelectedBrowserItemChanged(SystemBrovserItem value) {
		var cur = IoC.GetValue<string>("LastSelectedBrowser");
		if (cur != value.SystemBrowserType.ToString())
			IoC.SetValue(value.SystemBrowserType.ToString(), "LastSelectedBrowser");
	}
	partial void OnSelectedPlaywrightScriptChanged(RunScriptOptions value) {
		var cur = IoC.GetValue<string>("LastRunScriptId");
		if (value != null && cur != value.Description?.Title)
			IoC.SetValue(value.Description?.Title, "LastRunScriptId");
	}
	partial void OnSearchTextChanged(string value) {
		if (value.IsNot()) {
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

	private void SelectAll() {
		foreach (var profile in Profiles) {
			profile.IsSelected = true;
		}
	}

	[RelayCommand]
	private void UnselectItems() {
		foreach (var profile in Profiles) {
			profile.IsSelected = false;
		}
		PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
	}

	private async Task OpenSystemBrowser(SystemBrowserType browserType) {
		foreach (var profile in GetSelectedProfiles) {
			_ = await profile.OpenSystemBrowser(browserType);
		}
	}

	private async Task RunAutomation() {
		if (!GetSelectedProfiles.Any()) {
			Toaster.Error("Select one or more profiles to run the automation.");
			return;
		}

		IsVisibleRunButton = false;
		IsVisibleStopButton = true;
		var cts = new CancellationTokenSource();
		try {
			foreach (var profile in GetSelectedProfiles) {
				// Stop loop if canceled
				cts.Token.ThrowIfCancellationRequested();
				try {
					var description = SelectedPlaywrightScript.Description;
					if (
							description != null &&
							description.Parameters.TryGetValue("email", out var email) &&
							description.Parameters.TryGetValue("password", out var password) &&
							(email.Is() || email == "email" || password.Is() || password == "password")
						) {
						await UPAdditionalDataRepo.Instance.Loginz.Load();
						var google =
							description.Parameters["title"].Equals("google", StringComparison.CurrentCultureIgnoreCase) ||
							description.Parameters["website"].Equals("google.com", StringComparison.CurrentCultureIgnoreCase);
						if (!await TaskUtil.AwaitFor(() => profile.ProfileLogins.Count > 0, 4) && !google) {
							throw new Exception("No logins found in the profile.");
						}
						if (profile.ProfileLogins.Count > 0 || !google) {
							var login = profile.ProfileLogins.FirstOrDefault(l =>
								(l.title.IsNot() && l.title!.Equals(description.Parameters["title"], StringComparison.CurrentCultureIgnoreCase)) ||
								(l.WebSite.IsNot() && l.WebSite!.Equals(description.Parameters["website"], StringComparison.CurrentCultureIgnoreCase))
							) ?? profile.ProfileLogins[0];

							description.Parameters["email"] = login?.Email ?? email ?? string.Empty;
							description.Parameters["password"] = login?.Password ?? password ?? string.Empty;
						}
					}

					var browser = await profile.OpenSystemBrowser(SelectedBrowserItem.SystemBrowserType).WaitAsync(cts.Token);
					ArgumentNullException.ThrowIfNull(browser, nameof(browser));

					SelectedPlaywrightScript.Port = browser.Settings.Port;
					SelectedPlaywrightScript.Record = IsRecordSelected;
					await Run.Script(SelectedPlaywrightScript, cts.Token);
				} catch (Exception ex) {
					// Log or handle the exception if closing the process fails
					Toaster.Error($"{ex.Message}");
				}
			}
		} catch (Exception ex) {
			Toaster.Error($"{ex.Message}");
		} finally {
			StopAutomation();
			IsVisibleRunButton = true;
			IsVisibleStopButton = false;
			IsVisibleWaitButton = false;
		}
	}

	[RelayCommand]
	private void StopAutomation() {
		IsVisibleStopButton = false;
		IsVisibleWaitButton = true;
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
	}

	public static MyProfilesViewModel Instance { get; } = new MyProfilesViewModel();
}
