using Chameleon.app.Avalonia.Models;
using Chameleon.Avalonia.Common.Collections;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using System.Collections.ObjectModel;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Interfaces;
using Chameleon.lib.Playwright.Models;
using Chameleon.lib.WebBrowser.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using static Chameleon.lib.Common.Constants.Enums;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.Util;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Api.Repos;
using System.Reactive.Subjects;
using DynamicData;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.app.Avalonia.Controls;
using FluentAvalonia.Core;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;

public class SystemBrovserItemViewModel
		: ObservableObjectBase {
	public SystemBrovserItemViewModel(SystemBrowserType systemBrowserType)
	{
		SystemBrowserType = systemBrowserType;
	}

	private SystemBrowserType _systemBrowserType;
	public SystemBrowserType SystemBrowserType {
		get => _systemBrowserType;
		set => SetProperty(ref _systemBrowserType, value);
	}

	public string IconName => SystemBrowserType.ToString().ToLower();
}

public partial class UserProfilesViewModel : ViewModelObjectBase {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;

	private readonly ISysBrowserService _systemBrowserManager = IoC.GetService<ISysBrowserService>()!;
	private readonly IPlaywrightScriptRepository _plawrightRepository = IoC.GetService<IPlaywrightScriptRepository>()!;
	private readonly IPlaywriteService _playwriteService = IoC.GetService<IPlaywriteService>()!;

	private PaginatorViewModel? _paginatorViewModel;
	public AvList<PlaywrightScript> PlaywrightScripts { get; } = [];

	[ObservableProperty]
	private SystemBrovserItemViewModel? selectedBrowserItem;
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

	private CancellationTokenSource? _cts;
	private IEnumerable<ObsProfile>? _selectedProfiles;

	private List<ObsProfile> GetSelectedProfiles => _selectedProfiles!.ToList();
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public bool IsProfilesExist => Profiles.Any() == true;
	public bool HasNoItems => (!SearchText.Is()) || Profiles.Count == 0;
	public bool ShowFavoriteIcon => Folder?.id > 0;
	public bool HasProfileWithoutFolder => Profiles != null && Profiles.Any(profile => profile.Dto?.folderId != null);
	public bool IsSharedFolder => Folder != null && Folder?.creatorUserId != _authSession?.UserId && Folder?.creatorUserId != null;
	public string SelectedFolderTitle => Folder?.title ?? "All profiles";
	public ObservableCollection<SystemBrovserItemViewModel> BrowserItems { get; } = [
		new SystemBrovserItemViewModel(SystemBrowserType.Brave),
		new SystemBrovserItemViewModel(SystemBrowserType.Chrome)
	];

	//
	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.UserProfileVimCompares.AscendingComparer);
	private readonly ISubject<IPageRequest> pageRequests = new BehaviorSubject<IPageRequest>(new PageRequest(0, 25));

	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }

	private UserProfilesViewModel()
	{
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i, false))
			.SortAndPage(profilesCompareObservable, pageRequests)
						// no sort and bind options. These are extracted from the SortAndPage context
			.Bind(out var list)
			.Subscribe((i) => {
				OnHandleUserEvent();
				if (Profiles != null) {
					PaginatorViewModel ??= new PaginatorViewModel(Profiles.Count);
					PaginatorViewModel.TotalCount = Profiles.Count;
					TotalCount = PaginatorViewModel.TotalCount;
				}
			});
		Profiles = list;

		EventAggregator.Sub<AfterCreateOrRemoveFolderEvent>(OnHandleUserEvent);

		_ = EventAggregator.GetEvent<SelectedChangeUserProfileEvent>()
				.Subscribe(OnSelectedChanged);

		_ = EventAggregator.GetEvent<SavedUserProfileFolderEvent>()
				.Subscribe((e) => OnPropertyChanged(nameof(Folder)));
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		await InitializeScripts();
		InintializeLastSelectedAutomation();

		OnHandleUserEvent();
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		profilesCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.UserProfileVimCompares.DescendingComparer,
			_ => Compares.UserProfileVimCompares.AscendingComparer
		});
	}

	partial void OnSelectedBrowserItemChanged(SystemBrovserItemViewModel value)
	{
		var cur = IoC.GetValue<string>("LastSelectedBrowser");
		if (cur != value.SystemBrowserType.ToString())
			IoC.SetValue(value.SystemBrowserType.ToString(), "LastSelectedBrowser");
	}

	public bool IsSelectedScript => SelectedPlaywrightScript != null;
	private PlaywrightScript? _selectedPlaywrightScript;
	public PlaywrightScript? SelectedPlaywrightScript {
		get { return _selectedPlaywrightScript; }
		set {
			if (value != null && _selectedPlaywrightScript != value) {
				_ = SetProperty(ref _selectedPlaywrightScript, value);
				OnPropertyChanged(nameof(IsSelectedScript));
				RunAutomationCommand.NotifyCanExecuteChanged();

				var cur = IoC.GetValue<string>("LastRunScriptId");
				if (cur != value.Title)
					IoC.SetValue(value.Title, "LastRunScriptId");
			}
		}
	}

	private string _searchText = string.Empty;
	public string SearchText {
		get => _searchText;
		set {
			if (SetProperty(ref _searchText, value)) {
				ApplySearchFilter();
			}
		}
	}

	private Func<IUserProfile, bool>? _filter;
	public Func<IUserProfile, bool>? Filter {
		get => _filter;
		set {
			if (SetProperty(ref _filter, value)) {
				SetViewModelsFilter();
			}
		}
	}

	private UPFolderDto? _folder;
	public UPFolderDto? Folder {
		get {
			return _folder;
		}
		set {
			if (SetProperty(ref _folder, value)) {
				//UpdateFolder();
				SearchText = string.Empty;

				var folderId = value?.id;

				if (folderId == default) {
					HasFolder = false;
					Filter = null;

					return;
				}

				HasFolder = true;
				Filter = profile => profile.FolderId == folderId;
			}

			OnPropertyChanged(nameof(ShowFavoriteIcon));
			OnPropertyChanged(nameof(IsSharedFolder));
			OnPropertyChanged(nameof(SelectedFolderTitle));
		}
	}

	public PaginatorViewModel? PaginatorViewModel {
		get => _paginatorViewModel;
		set {
			if (SetProperty(ref _paginatorViewModel, value)) {
				_paginatorViewModel!.ChangePageIndex += (s, a) => { pageRequests.OnNext(new PageRequest(_paginatorViewModel.PageIndex, 25)); };
			}
		}
	}

	private int _selectedCount;
	public int SelectedCount {
		get => _selectedCount;
		set {
			if (SetProperty(ref _selectedCount, value)) {
				OnPropertyChanged(nameof(HasSelectedItems));
			}
		}
	}

	private async Task InitializeScripts()
	{
		void AddMappedScripts(IEnumerable<PlaywriteRunScriptOptions> scripts)
		{
			PlaywrightScripts.AddMapped(scripts, (Func<PlaywriteRunScriptOptions, PlaywrightScript>)(b => {
				var viewModel = new PlaywrightScript(b);
				viewModel.Parameters.AddRange((IEnumerable<PlaywrightDescriptionParam>)b.Description!.Parameters);
				return viewModel;
			}));
		}

		PlaywrightScripts.Clear();

		AddMappedScripts(_plawrightRepository.GetBundledScrits());

		var usd = IoC.GetValue<string>("UserScriptsDirectory");
		if (usd.Is() && Directory.Exists(usd)) {
			AddMappedScripts(await _plawrightRepository.GetUserScripts(usd));
		}

		OnPropertyChanged(nameof(SelectedBrowserItem));
	}

	private void InintializeLastSelectedAutomation()
	{
		var lastSelectedBrowserString = IoC.GetValue<string>("LastSelectedBrowser");

		SelectedBrowserItem = !lastSelectedBrowserString.Is() ||
				!Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum)
			? BrowserItems[0]
			: BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);

		SelectedPlaywrightScript = PlaywrightScripts.FirstOrDefault(s => s.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];
	}

	private void OnHandleUserEvent()
	{
		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(HasProfileWithoutFolder));
	}

	public void Open(UPFolderDto? folder)
	{
		Folder = folder;

		UnselectItems();
		OnHandleUserEvent();
	}

	private void OnSelectedChanged(SelectedUserProfileEventArgs? arr = null)
	{
		_selectedProfiles = Profiles.Where(profile => profile.IsSelected);
		SelectedCount = _selectedProfiles.Count();
	}

	[RelayCommand]
	private void SelectAll()
	{
		foreach (var profile in Profiles) {
			profile.IsSelected = true;
		}

		SelectedCount = Profiles.Count;
	}

	[RelayCommand]
	private void SelectAllProfilesFromFolder()
	{
		var profiles = Profiles
				.Where(p => p.Dto?.folderId == Folder?.id || Folder?.id == 0)
				.ToList();

		profiles.ForEach(p => p.IsSelected = true);
		SelectedCount = profiles.Count;
	}

	[RelayCommand]
	private void UnselectItems()
	{
		if (_selectedProfiles == null) {
			return;
		}

		foreach (var profile in _selectedProfiles) {
			profile.IsSelected = false;
		}
	}

	[RelayCommand]
	private async Task Delete()
	{
		if (_selectedProfiles == null) {
			return;
		}

		if (await Mbox.Show("Delete User Profiles",
				$"Are you sure you want to delete {SelectedCount} profiles?",
				MBoxButtons.OkCancel,
				"DeleteLines")) {
			var profiles = _selectedProfiles.ToList();

			foreach (var profile in profiles) {
				//await Task.Run(() => _userProfileService.Delete(profile.UserProfile));
				var res = await UserProfilesRepo.Instance.Delete(profile.Dto!.id);
				if (res.success) {
					profile.IsSelected = false;
				}
			}
			ChangeProfilesInFavoriteFolder();
			OnSelectedChanged();
			OnHandleUserEvent();
		}
	}

	[RelayCommand]
	private async Task RemoveProfilesFromFolder()
	{
		if (_folder?.id == 0 ||
				_selectedProfiles == null ||
				!_selectedProfiles.Any()) {
			return;
		}

		var ids = _selectedProfiles
				.Select(a => a.Dto!.id)
				.ToList();

		var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, null);
		if (res.success) {
			Filter = p => p.FolderId == _folder?.id;
			OnHandleUserEvent();
			ChangeProfilesInFavoriteFolder();
		}
	}

	[RelayCommand]
	private async Task AddProfilesToFolder()
	{
		if (Folder == null || Folder.id == 0)
			return;

		var addvm = new AddUserProfilesPupViewModel {
			Title = "Add Profiles"
		};

		PupUserProfileViewModel CreatePup(ObsProfile p)
		{
			var pup = new PupUserProfileViewModel(p);
			pup.OnSelectedChange += () => {
				addvm.SelectedViewModels = addvm.Profiles.Where(p => p.IsSelected).ToList();
			};
			return pup;
		}
		addvm.Profiles.AddRange(Profiles
							.Where(p => p.Dto!.folderId == null)
							.Select(CreatePup));
		

		if (await Mbox.ShowTaskDialog<AddUserProfilesPupViewModel, AddUserProfilesPopupUserControl>(() => addvm, 
			header: addvm.Title, 
			subHeader: $"Select profiles you want to add to {Folder!.title} folder:", 
			symbas: Enums.Symbas.Folder, 
			btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
			var ids = addvm.SelectedViewModels?
				.Select(a => a.UserProfile.Dto!.id)
				.ToList();
			if (ids == null || !ids.Any()) {
				return;
			}
			var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, Folder!.id);
			if (res.success) {
				//Filter = p => p.FolderId == _folder?.id;
				//OnHandleUserEvent();
			}
		}
	}

	[RelayCommand]
	private async Task MoveProfilesToFolder()
	{
		var selectedProfiles = Profiles
			.Where(p => p.IsSelected);
		if (!selectedProfiles.Any()) {
			return;
		}

		var addvm = new MoveUserProfilesPopupViewModel {
			Title = "Add To Folder"
		};

		addvm.Profiles.AddRange(selectedProfiles);


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

			var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, addvm.SelectedFolder.Dto!.id);
			if (res.success) {
				//Filter = p => p.FolderId == _folder?.id;
				//OnHandleUserEvent();
			}
		}
	}

	private void ChangeProfilesInFavoriteFolder()
	{
		var folderId = Folder?.id;
		if (folderId == null) return;

		EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Publish(new ChangeProfilesInFavoriteFolderEventArgs((int)folderId));
	}

	public async Task<UserProfileDto?> CreateNewProfile()
	{
		var folderId = HasFolder ? Folder?.id : null;

		var res = await UserProfilesRepo.CreateProfile($"New Profile - {Profiles.Count}", folderId);
		if (res != null) {
			OnHandleUserEvent();
		}

		return res;
	}


	[RelayCommand]
	private void OpenChameleonBrowser()
	{
		GetSelectedProfiles.ForEach(profile => {
			profile.OpenUserBrowser();
		});
	}

	[RelayCommand]
	private void OpenFirefox()
	{
		OpenSystemBrowser(SystemBrowserType.Firefox);
	}

	[RelayCommand]
	private void OpenChrome()
	{
		OpenSystemBrowser(SystemBrowserType.Chrome);
	}

	[RelayCommand]
	private void OpenBrave()
	{
		OpenSystemBrowser(SystemBrowserType.Brave);
	}

	private void OpenSystemBrowser(SystemBrowserType browserType)
	{
		GetSelectedProfiles.ForEach(async (selectedProfile) => {
			await selectedProfile.OpenSystemBrowser(browserType);
		});
	}

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

	[RelayCommand]
	private async Task RunAutomation()
	{
		if (!Profiles.Any(p => p.IsSelected == true)) {
			Toaster.ShowErr("Select one or more profiles to run the automation.");
			return;
		}
		if (SelectedPlaywrightScript == null) {
			Toaster.ShowErr("Select an automation.");
			return;
		}

		IsVisibleRunButton = false;
		IsVisibleStopButton = true;

		var token = RecreateCancellationToken;
		foreach (var profile in GetSelectedProfiles) {
			var browserWasNotOpened = profile.SBI![SelectedBrowserItem.SystemBrowserType] == null;
			if (browserWasNotOpened) {
				await profile.OpenSystemBrowser(SelectedBrowserItem.SystemBrowserType).WaitAsync(token);
				if (profile.SBI![SelectedBrowserItem.SystemBrowserType] == null || !await profile.SBI![SelectedBrowserItem.SystemBrowserType]!.LoadedTCS.Task.WaitAsync(token))
					continue;
			}
			var options = SelectedPlaywrightScript.RunOptions;
			options.Port = profile.SBI![SelectedBrowserItem.SystemBrowserType]!.Settings.Port;
			options.Record = IsRecordSelected;
			try {
				await _playwriteService.RunScript(SelectedPlaywrightScript.RunOptions, token);
			} catch (Exception ex) {
				// Log or handle the exception if closing the process fails
				Toaster.ShowErr($"{ex.Message}");
			}

			// Check if the browser process is not null and hasn't exited
			if (browserWasNotOpened) {
				await ProUtil.TryKillProcess(profile.SBI[SelectedBrowserItem.SystemBrowserType]?.Settings.Brocess);
			}

			// Stop loop if canceled
			if (token.IsCancellationRequested) {
				break;
			}
		}
		_playwriteService.Dispose();

		IsVisibleRunButton = true;
		IsVisibleStopButton = false;
		IsVisibleWaitButton = false;
	}


	[RelayCommand]
	private void StopAutomation()
	{
		IsVisibleStopButton = false;
		IsVisibleWaitButton = true;
		_cts?.Cancel();
	}

	private void SetViewModelsFilter()
	{
		//if (_viewModels == null) {
		//	return;
		//}

		//if (_filter == null) {
		//	_viewModels.Filter = null;
		//} else {
		//	_viewModels.Filter = (viewModel) => _filter(viewModel.UserProfile);
		//}

		//OnViewModelChange(this, EventArgs.Empty);
	}

	private void ApplySearchFilter()
	{
		var searchText = SearchText?.ToLower();
		var hasSearchText = !string.IsNullOrWhiteSpace(SearchText);
		var isInCurrentFolder = Folder?.creatorUserId != null;

		Filter = profile => FilterByFolder(profile, hasSearchText, isInCurrentFolder, searchText);

		OnPropertyChanged(nameof(ViewModels));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasNoItems));
	}

	private bool FilterByFolder(IUserProfile profile, bool hasSearchText, bool isInCurrentFolder, string searchText)
	{
		if (!hasSearchText && isInCurrentFolder) {
			return profile.FolderId == Folder?.id;
		}
		if (hasSearchText && isInCurrentFolder) {
			return profile.FolderId == Folder?.id && FilterByUserProfile(profile, searchText);
		}
		if (hasSearchText) {
			return FilterByUserProfile(profile, searchText);
		}

		return true;
	}

	private static bool FilterByUserProfile(IUserProfile profile, string searchText)
	{
		return profile.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);
	}

	public async void OnFilterTo(ObsProfile? p = null)
	{
		while (!Loaded)
			await Task.Delay(250);

		if (p != null) {
			if (p.Dto?.folderId is int fid && fid != 0)
				UserProfileFoldersViewModel.Instance.SetSelectedById(fid);
			else
				await UserProfileFoldersViewModel.Instance.OnNavigatingTo(null);

			//Filter = profile => p.Dto.id == profile.id;
		} else {
			//Filter = profile => 0 == profile.Id;
			UserProfileFoldersViewModel.Instance.SetSelectedById(0);
			Filter = null;
			//ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
		}

		OnHandleUserEvent();
	}

	public static UserProfilesViewModel Instance { get; } = new UserProfilesViewModel();
}
