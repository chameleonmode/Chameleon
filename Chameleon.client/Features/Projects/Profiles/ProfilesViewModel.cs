using Avalonia.Collections;

using DynamicData;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles.Dialogs;

using Chameleon.lib.Api.Dto;

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
	[ObservableProperty] bool isVisibleRunButton = true;
	[ObservableProperty] bool isVisibleStopButton;
	[ObservableProperty] bool isVisibleWaitButton;
	[ObservableProperty] bool isRecordSelected;
	[ObservableProperty] string searchText = string.Empty;

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
		pageRequests = new(new PageRequest(0, 9));
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(p => !HasFolder || p.Dto.folderId == Folder?.Id);
		_ = Shared
			.Filter(filter)
			.SortAndPage(AscendingComparer, pageRequests)
			.SortAndBind(out var profiles, CompareObservable)
			.Subscribe();
		Profiles = profiles;
		PaginatorViewModel = new PaginatorViewModel(p => pageRequests.OnNext(new PageRequest(p.CurrentIndex, p.OnPageItems))) {
			TotalCount = UserProfilesRepo.Instance.ObservableCache.Count,
		};
		InitializeCommands();
	}

	private void InitializeCommands() {
		void SelectAll() {
			foreach (var profile in Profiles) {
				profile.IsSelected = true;
			}
		}
		void StopAutomation() {
			IsVisibleStopButton = false;
			IsVisibleWaitButton = true;
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
		}
		CommandMap["StopAutomation"] = StopAutomation;
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
			GetSelectedProfiles?.ForEach(profile => SnapCracklePopViewModel.Open(profile.Dto));
			return Task.CompletedTask;
		};
		AsyncCommandMap["play"] = async () => {
			if (!GetSelectedProfiles.Any() || SelectedPlaywrightScript is null || SelectedBrowserOption is null) {
				Toaster.Error("Select one or more profiles to run the automation.");
				return;
			}
			void SetAutomationState(bool running) {
				IsVisibleRunButton = !running;
				IsVisibleStopButton = running;
				if (!running) {
					StopAutomation();
					IsVisibleWaitButton = false;
				}
			}
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
			async Task RunAutomationForProfile(ObsProfile profile, CancellationToken cancellationToken) {
				try {
					await ConfigureScriptParameters(profile);
					var browser = await profile.OpenSystemBrowser(SelectedBrowserOption.Option).WaitAsync(cancellationToken);

					SelectedPlaywrightScript!.Port = browser!.Settings.Port;
					SelectedPlaywrightScript.Record = IsRecordSelected;
					await Run.Script(SelectedPlaywrightScript, cancellationToken);
				} catch (Exception ex) {
					Toaster.Error($"{ex.Message}");
				}
			}
			SetAutomationState(true);
			cts = new CancellationTokenSource();

			try {
				foreach (var profile in GetSelectedProfiles) {
					cts.Token.ThrowIfCancellationRequested();
					await RunAutomationForProfile(profile, cts.Token);
				}
			} catch (Exception ex) {
				Toaster.Error($"{ex.Message}");
			} finally {
				SetAutomationState(false);
			}
		};
		AsyncCommandMap["Move"] = async () => {
			if (!GetSelectedProfiles.Any() ||
				await MoveProfilesPopup.Show(GetSelectedProfiles) is not { } mover) return;
			else _ = await UserProfilesRepo.MoveUserProfileToFolder(
				mover.Profiles.Select(a => a.Dto!.id), mover.SelectedFolder.Dto.id);

			ObsProfiles.ForEach(p => p.IsShowCheckboxColumn = true); // temp fix for now TODO: findout root cause
		};
		AsyncCommandMap["Remove"] = async () => {
			if (!GetSelectedProfiles.Any()) return;
			await UserProfilesRepo.MoveUserProfileToFolder(GetSelectedProfiles.Select(a => a.Dto!.id), null);
		};
		AsyncCommandMap["Delete"] = async () => {
			if (!GetSelectedProfiles.Any()) return;

			var confirmed = await MessageBox.Show(
				"Delete User Profiles",
				$"Are you sure you want to delete {SelectedCount} profiles?",
				MBoxButtons.OkCancel,
				"DeleteLines");

			if (!confirmed) return;

			foreach (var profile in GetSelectedProfiles.ToList()) {
				var result = await UserProfilesRepo.Instance.Delete(profile.Dto!.id);
				if (!result.success) profile.IsSelected = false;
			}
		};
		AsyncCommandMap["AddProfilesToFolder"] = async () => {
			if (!HasFolder) return;

			var addViewModel = new AddUserProfilesPupViewModel { Title = "Add Profiles" };

			var dialogResult = await MessageBox.ShowTaskDialog<AddUserProfilesPopupUserControl, AddUserProfilesPupViewModel>(new(
				Initialize: () => addViewModel,
				Header: addViewModel.Title,
				SubHeader: $"Select profiles you want to add to {Folder!.Title} folder:",
				Symbas: Symbas.Folder,
				Btns: MBoxButtons.OkCancel));

			if (dialogResult == TaskDialogResult.OK && addViewModel.SelectedProfiles.Any()) {
				await UserProfilesRepo.MoveUserProfileToFolder(
					addViewModel.SelectedProfiles.Select(o => o.Dto.id),
					Folder!.Id);
			}
		};
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		await InitializeScripts();
	}

	private async Task InitializeScripts() {
		PlaywrightScripts.Clear();
		PlaywrightScripts.AddRange(BundledScriptsService.Instance.GetBundledScrits());
		PlaywrightScripts.AddRange(await BundledScriptsService.GetUserScripts());
		SelectedPlaywrightScript ??= PlaywrightScripts[0];
	}

	partial void OnSearchTextChanged(string value) => SetViewModelsFilter();

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
		var count = UserProfilesRepo.Instance.ObservableCache.Items.Count;
		var baseName = "New Profile";
		var profileName = $"{baseName} - {count}";

		while (UserProfilesRepo.Instance.ObservableCache.Items.Any(i => i.title == profileName)) {
			profileName = $"{baseName} - {++count}";
		}

		var result = await UserProfilesRepo.CreateProfile(profileName, folderId);
		if (result != null) {
			SetViewModelsFilter();
		}
		return result;
	}

	public async void Filter(ObsProfile? p = null) {
		_ = await LoadedTCS.Task;
	}

	public override ObsProfile Deleted(ObsProfile profile) {
		SetViewModelsFilter();
		return base.Deleted(profile);
	}

	public override void SetViewModelsFilter() {
		PaginatorViewModel.UpdatePageCount(SearchText.Length > 3 ? MaxInFolderItems : 9);
		filter.OnNext(p =>
			(!HasFolder || p.Dto.folderId == Folder?.Id) &&
			(SearchText.Length < 3 || p.Title?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) == true)
		);

		RefreshProperties();
	}

	private void RefreshProperties() {
		OnPropertyChanged(nameof(TotalCount));
		OnPropertyChanged(nameof(HasFolder));
		OnPropertyChanged(nameof(HasProfiles));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedFolderTitle));
	}

	public static ProfilesViewModel Instance { get; } = new ProfilesViewModel();
}
