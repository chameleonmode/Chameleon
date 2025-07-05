using Avalonia.Collections;

using DynamicData;

using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright.Services;
using Chameleon.client.UI.Components.ViewModels;
using Chameleon.client.Features.Projects.Profiles.Dialogs;

using Chameleon.lib.Api.Dto;
using Avalonia.Markup.Xaml.MarkupExtensions;

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
	[ObservableProperty] UPFolderViewModel? folder;
	[ObservableProperty] Arguments? selectedPlaywrightScript;
	[ObservableProperty] string searchText = string.Empty;
	[ObservableProperty] bool automationing;

	readonly BehaviorSubject<Func<ObsProfile, bool>> filter;
	readonly BehaviorSubject<IPageRequest> pageRequests;
	CancellationTokenSource? cts;

	public override ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public PaginatorViewModel PaginatorViewModel { get; }
	public AvaloniaList<Arguments> PlaywrightScripts { get; } = [];

	public bool HasFolder => Folder?.Id > 0;
	public string SelectedFolderTitle => Folder?.Title ?? "x_x";
	public int TotalCount => PaginatorViewModel.TotalCount = MaxInFolderItems;
	public int MaxInFolderItems => HasFolder
		? UserProfilesRepo.Instance.ObservableCache.Items.Count(i => i.folderId == Folder?.Id)
		: UserProfilesRepo.Instance.ObservableCache.Count;

	public ProfilesViewModel() {
		ProfileUIContextManager.SetModuleContext(ProfileUIModule.Profiles, ProfileUIContext.Profiles);

		pageRequests = new(new PageRequest(0, 9));
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(p => !HasFolder || p.Dto.folderId == Folder?.Id);
		_ = Shared
			.Filter(filter)
			.SortAndPage(AscendingComparer, pageRequests)
			.Do(changeSet => {
				var profiles = changeSet.Select(c => c.Current);
				ProfileUIContextManager.ApplyContextToProfiles(profiles, ProfileUIContext.Profiles);
			})
			.SortAndBind(out var profiles, CompareObservable)
			.Subscribe();
		Profiles = profiles;
		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.PageIndex + 1, p.OnPageItems))) {
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
			PaginatorViewModel.UpdatePageCount(MaxInFolderItems);
			SelectAll();
		};
		CommandMap["UnselectItems"] = () => {
			Profiles.ForEach(p => p.IsSelected = false);
			PaginatorViewModel.UpdatePageCount(9);
		};
		AsyncCommandMap["SaveTags"] = () => TagsRepo.Instance.SaveTagsAsync(TagItemType.Folder, Folder!.Id.ToString(), Folder.Tags.ToTagsList());
		async Task OpenSystemBrowser(SystemBrowserType browserType) {
			foreach (var profile in SelectedProfiles) {
				_ = await profile.OpenSystemBrowser(browserType);
			}
		}
		AsyncCommandMap["chrome"] = () => OpenSystemBrowser(SystemBrowserType.Chrome);
		AsyncCommandMap["brave"] = () => OpenSystemBrowser(SystemBrowserType.Brave);
		AsyncCommandMap["firefox"] = () => OpenSystemBrowser(SystemBrowserType.Firefox);
		AsyncCommandMap["chameleon-logo"] = () => {
			SelectedProfiles?.ForEach(profile => SnapCracklePopViewModel.Open(profile.Dto));
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

				if (!await TaskUtil.AwaitFor(() => profile.ProfileLogins.Count > 0, 4)) {
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
					var browser = await profile.OpenSystemBrowser(SelectedBrowserOption.Option).WaitAsync(cts.Token);

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

		AsyncCommandMap["up-folder"] = async () => {
			if (await MoveProfilesPopup.Show(SelectedProfiles) is not { } mover) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(mover.Profiles.Select(a => a.Dto!.id), mover.SelectedFolder.Dto.id);
			SetViewModelsFilter();
		};
		AsyncCommandMap["minus-in-circle"] = async () => {
			if (!SelectedProfiles.Any()) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(SelectedProfiles.Select(a => a.Dto!.id), null);
			SetViewModelsFilter();
		};
		AsyncCommandMap["delete"] = async () => {
			if (!SelectedProfiles.Any() ||
			 !await MessageBox.Show(
				title: "Delete User Profiles",
				content: $"Are you sure you want to delete {SelectedCount} profiles?",
				icon: "DeleteLines")) return;

			foreach (var profile in SelectedProfiles.ToList()) {
				var result = await UserProfilesRepo.Instance.Delete(profile.Dto!.id);
				if (!result.success) profile.IsSelected = false;
			}
			PaginatorViewModel.CurrentIndex = 0;
			SetViewModelsFilter();
		};
		AsyncCommandMap["plus-in-circle"] = async () => {
			if (Folder is null ||
			 await AddProfilesPopup.Show(Folder) is not { } add ||
			 add.SelectedProfiles.Select(o => o.Dto.id) is not { } ids || !ids.Any()) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(ids, Folder.Id);
			SetViewModelsFilter();
		};
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
		
		await Task.Delay(20); // Allow reactive pipeline to update
		ProfileUIContextManager.ApplyContextToProfiles(Profiles, ProfileUIContext.Profiles);
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
		PaginatorViewModel.TotalCount = MaxInFolderItems;
		PaginatorViewModel.UpdatePageCount(SearchText.Length > 3 ? MaxInFolderItems : MaxInFolderItems > 0 ? 9 : 1);
		filter.OnNext(p =>
			(!HasFolder || p.Dto.folderId == Folder?.Id) &&
			(SearchText.Length < 3 || p.Title?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) == true)
		);
		RefreshProperties();

		_ = Task.Run(async () => {
			await Task.Delay(10); // Small delay to let reactive chain update
			ProfileUIContextManager.ApplyContextToProfiles(Profiles, ProfileUIContext.Profiles);
		});
	}

	private void RefreshProperties() {
		OnPropertyChanged(nameof(TotalCount));
		OnPropertyChanged(nameof(HasFolder));
		OnPropertyChanged(nameof(HasProfiles));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedFolderTitle));
	}

	public void ApplyProfilesContext() {
		// Apply Profiles context when returning to the main profiles view
		ProfileUIContextManager.ApplyContextToProfiles(ObsProfiles, ProfileUIContext.Profiles);
	}

	public static ProfilesViewModel Instance { get; } = new ProfilesViewModel();
}
