using Avalonia.Collections;

using DynamicData;
using DynamicData.Binding;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Chameleon.lib.Util;
using Chameleon.lib;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.Util;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.client.UI.Components.ViewModels;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles.Dialogs;
using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class UPFolderViewModel : ObservableObjectBase {
	public UPFolderViewModel(UPFolderDto folder) {
		Id = folder.id;
		Title = folder.title;
		Tags = folder.Tags;
		IsFavorite = folder.isFavorite;
		ProfilesCount = folder.profilesCount;
		CreatorUserId = folder.creatorUserId;
	}

	[ObservableProperty] int id;
	[ObservableProperty] string? title;
	[ObservableProperty] bool isFavorite;
	[ObservableProperty] int profilesCount;
	[ObservableProperty] long? creatorUserId;
	[ObservableProperty] string? tags;

	public UPFolderDto ToDto() {
		return new UPFolderDto() {
			id = Id,
			title = Title,
			Tags = Tags,
			isFavorite = IsFavorite,
			profilesCount = ProfilesCount,
			creatorUserId = CreatorUserId
		};
	}
}

public partial class ProfilesViewModel : Projector {
	public static SortExpressionComparer<ObsProfile> AscendingComparer => SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsProfile> DescendingComparer => SortExpressionComparer<ObsProfile>.Descending(p => p.Dto!.title!);
	readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(AscendingComparer);
	readonly BehaviorSubject<IPageRequest> pageRequests = new(new PageRequest(0, 9));
	readonly BehaviorSubject<Func<ObsProfile, bool>> filter;

	CancellationTokenSource? cts;

	[ObservableProperty] Arguments selectedPlaywrightScript;
	[ObservableProperty] PaginatorViewModel paginatorViewModel;
	[ObservableProperty] BrowserOption selectedBrowserItem;
	[ObservableProperty] int totalCount;
	[ObservableProperty] bool isVisibleRunButton = true;
	[ObservableProperty] bool isVisibleStopButton;
	[ObservableProperty] bool isVisibleWaitButton;
	[ObservableProperty] bool isRecordSelected;
	[ObservableProperty] string searchText = string.Empty;
	[ObservableProperty] UPFolderViewModel? folder;
	[ObservableProperty] ChangeComparereOption sortSelected = ChangeComparereOption.Ascending;

	public AvaloniaList<Arguments> PlaywrightScripts { get; } = [];
	public ObservableCollection<BrowserOption> BrowserItems { get; } = [
		new BrowserOption(SystemBrowserType.Chrome),
		new BrowserOption(SystemBrowserType.Brave),
	];
	public ChangeComparereOption[] Sorts { get; } = (ChangeComparereOption[])Enum.GetValues(typeof(ChangeComparereOption));

	public bool HasFolder => Folder != null && Folder.Id != 0;
	public string SelectedFolderTitle => Folder?.Title ?? "x_x";
	public int SelectedCount => GetSelectedProfiles?.Count() ?? 0;
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	private IEnumerable<ObsProfile> GetSelectedProfiles => Profiles.Where(i => i.IsSelected);
	public bool HasProfileWithoutFolder => Profiles != null && Profiles.Any(profile => profile.Dto?.folderId != null);
	public int MaxInFolderItems =>
	Folder == null || Folder.Id == 0
	? UserProfilesRepo.Instance.ObservableCache.Count
	: UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == Folder.Id);

	public ProfilesViewModel() {
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(p => Folder == null || Folder.Id == 0 || p.Dto.folderId == Folder.Id);
		_ = UserProfilesRepo.Connect()
		.Transform(i => new ObsProfile(i,
			onSelectedChanged: p => {
				OnPropertyChanged(nameof(HasSelectedItems));
				OnPropertyChanged(nameof(SelectedCount));
			},
			onDeleted: p => SetViewModelsFilter()))
		.Filter(filter)
		.SortAndPage(AscendingComparer, pageRequests)
		.SortAndBind(out var profiles, profilesCompareObservable).Subscribe();
		Profiles = profiles;

		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		TotalCount = PaginatorViewModel.TotalCount;

		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		SelectedPlaywrightScript =
			PlaywrightScripts.FirstOrDefault(s => s.Description?.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];

		SelectedBrowserItem = BrowserItems[0];

		CommandMap["SelectAll"] = SelectAll;
		CommandMap["SelectAllProfilesFromFolder"] = () => {
			PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
			SelectAll();
		};
		CommandMap["UnselectItems"] = () => {
			Profiles.ForEach(p => p.IsSelected = false);
			PaginatorViewModel.UpdatePageCount(9);
		};

		AsyncCommandMap["SaveTags"] = () => TagsRepo.Instance.SaveTagsAsync(TagItemType.Folder, Folder!.Id.ToString(), Folder.Tags.ToTagsList());
		AsyncCommandMap["chrome"] = () => OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["brave"] = () => OpenSystemBrowser(SystemBrowserType.Brave);
		AsyncCommandMap["firefox"] = () => OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["hwinds"] = () => {
			GetSelectedProfiles?.ForEach(profile => {
				profile.OpenTopmostController();
			});
			return Task.CompletedTask;
		};
		AsyncCommandMap["play"] = async () => {
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

						var browser = await profile.OpenSystemBrowser(SelectedBrowserItem.Option).WaitAsync(cts.Token);
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
		};
		AsyncCommandMap["Move"] = async () => {
			if (!GetSelectedProfiles.Any()) return;

			var addvm = new MoveUserProfilesPopupViewModel {
				Title = "Add To Folder"
			};
			addvm.Profiles.AddRange(GetSelectedProfiles);
			addvm.Folders.AddRange(FoldersViewModel.Instance.Folders);

			if (
				await MessageBox.ShowTaskDialog<MoveUserProfilesPopupUserControl, MoveUserProfilesPopupViewModel>(new(
					Initialize: () => addvm,
					Header: addvm.Title,
					SubHeader: $"Select a folder to move the {GetSelectedProfiles.Count()} selected profiles:",
					Symbas: Symbas.Folder,
					Btns: MBoxButtons.OkCancel)) == TaskDialogResult.OK &&
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
				await MessageBox.Show(
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
				await MessageBox.ShowTaskDialog<AddUserProfilesPopupUserControl, AddUserProfilesPupViewModel>(new(
					Initialize: () => addvm,
					Header: addvm.Title,
					SubHeader: $"Select profiles you want to add to {Folder!.Title} folder:",
					Symbas: Symbas.Folder,
					Btns: MBoxButtons.OkCancel)) == TaskDialogResult.OK
			) {
				if (!addvm.SelectedProfiles.Any()) return;
				_ = await UserProfilesRepo.MoveUserProfileToFolder(addvm.SelectedProfiles.Select(o => o.Dto.id), Folder!.Id);
			}

			SetViewModelsFilter();
		};
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		Profiles.ForEach(p => p.IsActionOptionsVisible = p.IsShowCheckboxColumn = true);
		PaginatorViewModel.UpdatePageCount(9);

		PlaywrightScripts.Clear();
		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		var usd = IoC.GetValue<string>("UserScriptsDirectory");
		if (usd.IsNot() && Directory.Exists(usd)) {
			PlaywrightScripts.AddRange(await BundledScriptsService.GetUserScripts(usd));
		}

		SelectedBrowserItem =
		Enum.TryParse<SystemBrowserType>(IoC.GetValue<string>("LastSelectedBrowser"), out var browserEnum)
		? BrowserItems.FirstOrDefault(b => b.Option == browserEnum) ?? BrowserItems[0]
		: BrowserItems[0];

		SelectedPlaywrightScript = PlaywrightScripts
		.FirstOrDefault(s => s.Description?.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];
	}

	partial void OnSortSelectedChanged(ChangeComparereOption value) {
		profilesCompareObservable.OnNext(value switch {
			ChangeComparereOption.Descending => DescendingComparer,
			_ => AscendingComparer
		});
	}
	partial void OnSelectedBrowserItemChanged(BrowserOption value) {
		var cur = IoC.GetValue<string>("LastSelectedBrowser");
		if (cur != value.Option.ToString())
			IoC.SetValue(value.Option.ToString(), "LastSelectedBrowser");
	}
	partial void OnSelectedPlaywrightScriptChanged(Arguments value) {
		var cur = IoC.GetValue<string>("LastRunScriptId");
		if (value != null && cur != value.Description?.Title)
			IoC.SetValue(value.Description?.Title, "LastRunScriptId");
	}
	partial void OnSearchTextChanged(string value) {
		if (value.IsNot()) {
			PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
			filter.OnNext(p =>
			p.Title?.Contains(value, StringComparison.CurrentCultureIgnoreCase) == true &&
			(Folder == null || Folder.Id == 0 || (Folder != null && Folder.Id != 0 && p.Dto?.folderId == Folder?.Id)));
		} else {
			PaginatorViewModel.UpdatePageCount(9);
			filter.OnNext(filter.Value);
		}

		SetViewModelsFilter(false);
	}

	private async Task OpenSystemBrowser(SystemBrowserType browserType) {
		foreach (var profile in GetSelectedProfiles) {
			_ = await profile.OpenSystemBrowser(browserType);
		}
	}

	public async Task OpenAsync(UPFolderDto? folder) {
		if (folder is not null) {
			Folder = new UPFolderViewModel(folder);
			Folder.Tags = await TagsRepo.Instance.GetTagsAsync(TagItemType.Folder, Folder.Id.ToString()).ToStringAsync();
			SearchText = string.Empty;
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

		if (p?.Dto.folderId is int fid && fid != 0) FoldersViewModel.Instance.SetSelectedById(fid);
		else if (p != null) await FoldersViewModel.Instance.OnNavigatingTo(null);
		else FoldersViewModel.Instance.SetSelectedById(0);

		SearchText = p?.Title ?? string.Empty;
	}

	public void SetViewModelsFilter(bool onext = true) {
		if (onext) filter.OnNext(filter.Value);

		TotalCount = PaginatorViewModel.TotalCount = MaxInFolderItems;

		OnPropertyChanged(nameof(HasFolder));
		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedFolderTitle));
		OnPropertyChanged(nameof(HasProfileWithoutFolder));
	}

	private void SelectAll() {
		foreach (var profile in Profiles) {
			profile.IsSelected = true;
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

	[RelayCommand]
	void UnselectItems() {
		CommandMap["UnselectItems"]();
	}

	public static ProfilesViewModel Instance { get; } = new ProfilesViewModel();
}
