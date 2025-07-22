using Avalonia.Collections;

using DynamicData;

using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Chameleon.lib.Util;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.client.UI.Components.ViewModels;
using Chameleon.client.Features.Projects.Profiles.Dialogs;

using Chameleon.lib.Api.Dto;
using Chameleon.lib.Browzio;

namespace Chameleon.client.Features.Projects.Profiles;

public partial class AvailableBrowser(BrowserInfo info) : ObservableObject {
	//public byte[]? IconData { get; } = IconExtractor.ExtractIcon(ExecutablePath);
	[ObservableProperty] int running;
	public BrowserInfo Info { get; } = info;
}
public partial class UPFolderViewModel : OO {
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

	public UPFolderDto ToDto() => new() {
		id = Id,
		title = Title,
		Tags = Tags,
		isFavorite = IsFavorite,
		profilesCount = ProfilesCount,
		creatorUserId = CreatorUserId
	};
}

public partial class ProfilesViewModel : Profiler {
	public const int DefaultPageSize = 9;
	[ObservableProperty] UPFolderViewModel? folder;
	[ObservableProperty] Arguments? selectedPlaywrightScript;
	[ObservableProperty] string searchText = string.Empty;
	[ObservableProperty] bool automationing;

	readonly HashSet<int> selectedProfileIds = [];
	readonly BehaviorSubject<Func<ObsProfile, bool>> filter;
	readonly BehaviorSubject<IPageRequest> pageRequests;
	CancellationTokenSource? cts;

	public override ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<AvailableBrowser> Browsers { get; } = [];
	public PaginatorViewModel Paginator { get; }
	public AvaloniaList<Arguments> PlaywrightScripts { get; } = [];

	public override bool HasSelectedItems => selectedProfileIds.Count > 0;
	public override int SelectedCount => selectedProfileIds.Count;
	public override IEnumerable<ObsProfile> SelectedProfiles => 
		UserProfilesRepo.Instance.ObservableCache.Items
			.Where(dto => selectedProfileIds.Contains(dto.id))
			.Select(dto => ObsProfiles.First(p => p.Dto.id == dto.id))
			.ToList();
	public bool HasFolder => Folder?.Id > 0;
	public string SelectedFolderTitle => Folder?.Title ?? "x_x";
	public int TotalCount => Paginator.TotalCount = MaxInFolderItems;
	public int MaxInFolderItems => HasFolder
		? UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == Folder?.Id)
		: UserProfilesRepo.Instance.ObservableCache.Count;

	public ProfilesViewModel() {
		pageRequests = new(new PageRequest(0, DefaultPageSize));
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(p => !HasFolder || p.Dto.folderId == Folder?.Id);

		_ = Shared.Filter(filter)
			.SortAndPage(AscendingComparer, pageRequests)
			.Do(changeSet => {
				var profiles = changeSet.Select(c => c.Current);
				ProfileUIContextManager.ApplyContextToProfiles(profiles, ProfileUIContext.Profiles);

				foreach (var profile in profiles) {
					profile.IsSelected = selectedProfileIds.Contains(profile.Dto.id);
					profile.OnSelectedChanged += p => SelectedChanged((ObsProfile)p);
				}
			})
			.SortAndBind(out var profiles, CompareObservable)
			.Subscribe();
		Profiles = profiles;

		Browsers.AddRange(
			Browzio.Utilities.DetectBrowsers()
			.Where(b => b.Engine == BrowserEngine.Chromium || b.Type == BrowserType.Firefox)
			.Select(b => new AvailableBrowser(b))
		);

		Paginator = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.PageIndex + 1, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		InitializeCommands();
	}

	private void InitializeCommands() {
		void SelectAll() {
			foreach (var profile in Profiles) {
				profile.IsSelected = true;
			}
			OnPropertyChanged(nameof(HasSelectedItems));
		}
		CommandMap["select-all"] = SelectAll;
		CommandMap["select-folder"] = () => {
			foreach (var profile in Profiles) {
				profile.IsSelected = true;
			}

			if (HasFolder) {
				// Get all profile DTOs in this folder and mark them as selected internally
				var profileIdsInFolder = UserProfilesRepo.Instance.ObservableCache.Items
					.Where(dto => dto.folderId == Folder?.Id)
					.Select(dto => dto.id)
					.ToList();

				// Store these IDs for use when other pages are loaded
				selectedProfileIds.UnionWith(profileIdsInFolder);
			} else {
				// Select all profiles
				var allProfileIds = UserProfilesRepo.Instance.ObservableCache.Items
					.Select(dto => dto.id)
					.ToList();

				selectedProfileIds.UnionWith(allProfileIds);
			}

			OnPropertyChanged(nameof(HasSelectedItems));
			OnPropertyChanged(nameof(SelectedCount));
		};
		CommandMap["UnselectItems"] = () => {
			selectedProfileIds.Clear();
			Profiles.ForEach(p => p.IsSelected = false);
			Paginator.UpdatePageCount(DefaultPageSize); // Reset page count to default
			// Force a refresh to make sure the selection state is properly updated
			SetViewModelsFilter();
		};

		AsyncCommandMap["SaveTags"] = () => TagsRepo.Instance.SaveTagsAsync(TagItemType.Folder, Folder!.Id.ToString(), Folder.Tags.ToTagsList());

		AsyncCommandMap["up-folder"] = async () => {
			if (await MoveProfilesPopup.Show(SelectedProfiles) is not { } mover) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(mover.Profiles.Select(a => a.Dto!.id), mover.SelectedFolder.Dto.id);
			selectedProfileIds.Empty(i => mover.Profiles.Any(p => p.Dto.id == i));

			// Update the view and notify about selection changes
			SetViewModelsFilter();
		};
		AsyncCommandMap["minus-in-circle"] = async () => {
			if (!SelectedProfiles.Any()) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(SelectedProfiles.Select(a => a.Dto!.id), null);
			SetViewModelsFilter();
		};
		AsyncCommandMap["delete"] = async () => {
			if (!SelectedProfiles.Any() ||
			 	await MessageBox.Show("Delete?", $"Are you sure you want to delete {SelectedCount} profiles?", icon: "DeleteLines") == false
			) return;

			await selectedProfileIds.Empty(async id => {
				var result = await UserProfilesRepo.Instance.Delete(id);
				return result.success;
			});
			Paginator.CurrentIndex = Paginator.CurrentIndex > Paginator.PageCount ? Paginator.PageCount : Paginator.CurrentIndex;
			SetViewModelsFilter();
		};
		AsyncCommandMap["plus-in-circle"] = async () => {
			if (Folder is null ||
			 await AddProfilesPopup.Show(Folder) is not { } add ||
			 add.SelectedProfiles.Select(o => o.Dto.id) is not { } ids || !ids.Any()) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(ids, Folder.Id);
			SetViewModelsFilter();
		};

		async Task OpenBrowser(BrowserType bt) {
			await SelectedProfiles.TryEach(async profile => {
				await profile.AsyncCfVCommand.ExecuteAsync(bt.ToString());
				await Task.Delay(300); // Small delay to prevent rapid opening
			});
		}
		foreach (var browser in Browsers) {
			AsyncCommandMap[browser.Info.Type.ToString()] = () => OpenBrowser(browser.Info.Type);
		}
		AsyncCommandMap["chrome"] = () => OpenBrowser(BrowserType.Chrome);
		AsyncCommandMap["brave"] = () => OpenBrowser(BrowserType.Brave);
		AsyncCommandMap["firefox"] = () => OpenBrowser(BrowserType.Firefox);
		AsyncCommandMap["chameleon-logo"] = () => {
			SelectedProfiles?.ForEach(profile => SnapCracklePopViewModel.Open(profile));
			return Task.CompletedTask;
		};

		void StopAutomation() {
			Automationing = false;
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}
		async Task StartAutomation(bool record) {
			if (!SelectedProfiles.Any()) throw new InvalidOperationException("No profiles selected for automation.");
			Automationing = true;
			async Task ConfigureScriptParameters(ObsProfile profile) {
				if (SelectedPlaywrightScript!.Description is not ScriptDescription description) return;

				bool RequiresLoginCredentials() => description.Parameters.ContainsKey("email") && description.Parameters.ContainsKey("password");
				if (!RequiresLoginCredentials()) return;

				await UPAdditionalDataRepo.Instance.Loginz.Load();
				bool IsGoogleLogin() =>
					description.Parameters.GetValueOrDefault("title", "").Equals("google", StringComparison.CurrentCultureIgnoreCase) ||
					description.Parameters.GetValueOrDefault("website", "").Equals("google.com", StringComparison.CurrentCultureIgnoreCase);

				if (await EX.Poly<bool>(() => Task.FromResult(profile.ProfileLogins.Count > 0 ? true : throw new Exception())) == true) {
					throw new Exception("No logins found in the profile.");
				}
				UPLoginDto? FindMatchingLogin() =>
					profile.ProfileLogins.FirstOrDefault(l =>
						(l.title?.Equals(description.Parameters.GetValueOrDefault("title"), StringComparison.CurrentCultureIgnoreCase) == true
						|| (IsGoogleLogin() && l.title?.Equals("google", StringComparison.CurrentCultureIgnoreCase) == true)) &&
						(l.WebSite?.Equals(description.Parameters.GetValueOrDefault("website"), StringComparison.CurrentCultureIgnoreCase) == true
						|| (IsGoogleLogin() && l.WebSite?.Equals("google.com", StringComparison.CurrentCultureIgnoreCase) == true)));

				var login = FindMatchingLogin() ?? profile.ProfileLogins[0];
				description.Parameters["email"] = login?.Email ?? description.Parameters.GetValueOrDefault("email", string.Empty);
				description.Parameters["password"] = login?.Password ?? description.Parameters.GetValueOrDefault("password", string.Empty);
			}

			cts = new CancellationTokenSource();
			try {
				foreach (var profile in SelectedProfiles) {
					cts.Token.ThrowIfCancellationRequested();
					await ConfigureScriptParameters(profile);
					await profile.AsyncCfVCommand.ExecuteAsync(SelectedBrowserOption.Option.ToString()).WaitAsync(cts.Token);
					var browser = profile.SBI[SelectedBrowserOption.Option];
					SelectedPlaywrightScript!.Port = browser!.Settings.Port;
					SelectedPlaywrightScript.Record = record;
					await Run.Script(SelectedPlaywrightScript, cts.Token);
				}
			} catch (Exception ex) {
				Toaster.Error($"{ex.Message}");
			} finally {
				StopAutomation();
			}
		}
		CommandMap["Stop"] = StopAutomation;
		AsyncCommandMap["Record"] = async () => await StartAutomation(true);
		AsyncCommandMap["Play"] = async () => await StartAutomation(false);
	}

	public override void SelectedChanged(ObsProfile profile) {
		if (profile.Dto == null) return;
		_ = profile.IsSelected ? selectedProfileIds.Add(profile.Dto.id) : selectedProfileIds.Remove(profile.Dto.id);
		OnPropertyChanges();
	}

	public override async Task Init(object? param) {
		await base.Init(param);
		// await InitializeScripts();
		PlaywrightScripts.Clear();
		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		PlaywrightScripts.AddRange(await BundledScriptsService.GetUserScripts());
		SelectedPlaywrightScript ??= PlaywrightScripts[0];
	}

	partial void OnSearchTextChanged(string value) => SetViewModelsFilter();

	public async Task OpenAsync(UPFolderDto folder) {
		Folder = new UPFolderViewModel(folder);
		Folder.Tags = await TagsRepo.Instance.GetTagsAsync(TagItemType.Folder, Folder.Id.ToString()).ToStringAsync();
		SearchText = string.Empty;
		SetViewModelsFilter();
	}

	public async Task<UserProfileDto?> CreateNewProfile() {
		var result = await UserProfilesRepo.CreateProfile(folderId: Folder?.Id);
		if (result != null) SetViewModelsFilter();
		return result;
	}

	public override ObsProfile Deleted(ObsProfile profile) {
		SetViewModelsFilter();
		return base.Deleted(profile);
	}

	public void SetViewModelsFilter() {
		Paginator.TotalCount = MaxInFolderItems;
		Paginator.UpdatePageCount(SearchText.Length > 3 ? MaxInFolderItems : MaxInFolderItems > 0 ? DefaultPageSize : 1);
		filter.OnNext(p =>
			(!HasFolder || p.Dto.folderId == Folder?.Id) &&
			(SearchText.Length < 3 || p.Title?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) == true)
		);

		_ = Task.Run(async () => {
			await Task.Delay(10); // Small delay to let reactive chain update
			ProfileUIContextManager.ApplyContextToProfiles(Profiles, ProfileUIContext.Profiles);
		});
		OnPropertyChanges();
	}

	public override void OnPropertyChanges() {
		base.OnPropertyChanges();
		OnPropertyChanged(nameof(HasFolder));
		OnPropertyChanged(nameof(TotalCount));
		OnPropertyChanged(nameof(SelectedFolderTitle));
	}

	public void ApplyProfilesContext() {
		// Apply Profiles context when returning to the main profiles view
		ProfileUIContextManager.ApplyContextToProfiles(ObsProfiles, ProfileUIContext.Profiles);
	}

	public static ProfilesViewModel Instance { get; } = new ProfilesViewModel();
}
