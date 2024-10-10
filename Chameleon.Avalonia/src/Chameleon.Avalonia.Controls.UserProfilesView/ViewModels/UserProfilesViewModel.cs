using Chameleon.app.Avalonia.Models.Playwright;
using Chameleon.app.Avalonia.ViewModels.Playwright;
using Chameleon.Avalonia.Common.Collections;
using Chameleon.Avalonia.Controls.Paginator.ViewModels;
using Chameleon.Common.Helpers;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Core.Extensions;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.App.UserProfiles.Views.List;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Extensions;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.Playwright.Interfaces;
using Chameleon.lib.Playwright.Models;
using Chameleon.lib.WebBrowser.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.ComponentModel;

using static Chameleon.lib.Common.Constants.Enums;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public partial class UserProfilesViewModel
		: SubPageViewModelBase
		, IUserProfilesViewModel {
	private readonly IUserProfileService _userProfileService;
	private readonly IUserProfileFolderService _userProfileFolderService;
	private readonly IApplicationUser _currentUser;
	private readonly ISysBrowserService _systemBrowserManager;
	private readonly ObservableCollection<SystemBrovserItemViewModel> _browserItems =
[
		new SystemBrovserItemViewModel(SystemBrowserType.Brave),
				new SystemBrovserItemViewModel(SystemBrowserType.Chrome)
];

	private ObservableCollection<IUserProfile, UserProfileViewModel> _mapping;
	private IEnumerable<UserProfileViewModel> _selectedProfiles;

	private SystemBrovserItemViewModel _selectedBrowserItem;
	private CancellationTokenSource _cts;

	[ObservableProperty]
	private int _totalCount;

	[ObservableProperty]
	private bool _isWaiting = true;

	[ObservableProperty]
	private bool _hasFolder;

	[ObservableProperty]
	private bool _isVisibleRunButton = true;

	[ObservableProperty]
	private bool _isVisibleStopButton;

	[ObservableProperty]
	private bool _isVisibleWaitButton;

	[ObservableProperty]
	private bool _isRecordSelected;

	private List<IUserProfileActionsViewModel> GetSelectedProfiles => _selectedProfiles.Cast<IUserProfileActionsViewModel>().ToList();
	public bool HasSelectedItems => ViewModels != null && ViewModels.Any(v => v.IsSelected);
	public bool IsProfilesExist => _mapping?.Any() == true;
	public bool HasNoItems => !SearchText.HasAny() && ViewModels == null || ViewModels.Count == 0;
	public bool ShowFavoriteIcon => Folder?.Id > 0;
	public bool HasProfileWithoutFolder => _mapping != null && _mapping.Any(profile => !profile.UserProfile.FolderId.HasValue);
	public IApplicationUser CurrentUser => _currentUser;
	public bool IsAddProfilesToFolderCommandEnabled => HasProfileWithoutFolder && !CurrentUser.IsAssistant && Folder?.Id != 0;
	public bool IsSharedFolder => Folder != null && _userProfileFolderService.IsSharedFolder(Folder);
	public string SelectedFolderTitle => Folder?.Title ?? "All profiles";
	public ObservableCollection<SystemBrovserItemViewModel> BrowserItems => _browserItems;

	public SystemBrovserItemViewModel SelectedBrowserItem {
		get => _selectedBrowserItem;
		set {
			_ = SetProperty(ref _selectedBrowserItem, value);
			var cur = lib.Common.IoC.GetValue<string>("LastSelectedBrowser");
			if (cur != value.SystemBrowserType.ToString())
				lib.Common.IoC.SetValue(value.SystemBrowserType.ToString(), "LastSelectedBrowser");
		}
	}

	public AvList<AutomationScriptModel> PlaywrightScripts { get; } = [];

	public bool IsSelectedScript => SelectedPlaywrightScript != null;
	private AutomationScriptModel? _selectedPlaywrightScript;
	public AutomationScriptModel? SelectedPlaywrightScript {
		get { return _selectedPlaywrightScript; }
		set {
			if (value != null && _selectedPlaywrightScript != value) {
				_ = SetProperty(ref _selectedPlaywrightScript, value);
				OnPropertyChanged(nameof(IsSelectedScript));
				RunAutomationCommand.NotifyCanExecuteChanged();

				var cur = lib.Common.IoC.GetValue<string>("LastRunScriptId");
				if(cur != value.Title)
					lib.Common.IoC.SetValue(value.Title, "LastRunScriptId");
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
	public ListSortDirection[] Sorts { get; } = (ListSortDirection[])Enum.GetValues(typeof(ListSortDirection));

	private ListSortDirection _sortSelected = ListSortDirection.Ascending;
	public ListSortDirection SortSelected {
		get => _sortSelected;
		set {
			if (SetProperty(ref _sortSelected, value)) {
				ViewModels.Ascending = value == ListSortDirection.Ascending;
				PaginatorViewModel.PageIndex = 0;
			}
		}
	}

	private IUserProfileFolder? _folder;
	public IUserProfileFolder? Folder {
		get {
			return _folder;
		}
		set {
			if (SetProperty(ref _folder, value)) {
				//UpdateFolder();
				SearchText = string.Empty;

				int folderId = Folder.Id;

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

	private ObservableCollectionView<UserProfileViewModel>? _viewModels;
	public ObservableCollectionView<UserProfileViewModel> ViewModels {
		get {
			if (_viewModels == null && _mapping != null) {
				_viewModels = new ObservableCollectionView<UserProfileViewModel>(_mapping) {
					TrackItemChanges = true,
					TrackCollectionChanges = true,
					Order = profile => profile.Title
				};

				//InitPaginator();
				PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
				ViewModels.Offset = PaginatorViewModel.Skip;
				ViewModels.Limit = PaginatorViewModel.OnPageItems;
				TotalCount = PaginatorViewModel.TotalCount;
				SetViewModelsFilter();
				OnPropertyChanged(nameof(HasNoItems));
				OnPropertyChanged(nameof(HasSelectedItems));
			}

			return _viewModels!;
		}
	}

	private PaginatorViewModel _paginatorViewModel;
	public PaginatorViewModel PaginatorViewModel {
		get => _paginatorViewModel;
		set {
			if (SetProperty(ref _paginatorViewModel, value)) {
				_paginatorViewModel.ChangePageIndex += (s, a) => { ViewModels.Offset = PaginatorViewModel.Skip; };
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

	private readonly IPlaywrightScriptRepository _plawrightRepository;
	private readonly IPlaywriteService _playwriteService;
	public UserProfilesViewModel(
			IUserProfileService userProfileService,
			IUserProfileFolderService userProfileFolderService,
			ISysBrowserService systemBrowserManager,
			IApplicationUser currentUser,
			IPlaywrightScriptRepository plawrightRepository,
			IPlaywriteService playwriteService)
	{
		_playwriteService = playwriteService;
		_plawrightRepository = plawrightRepository;
		_systemBrowserManager = systemBrowserManager;
		_userProfileService = userProfileService;
		_userProfileFolderService = userProfileFolderService;
		_currentUser = currentUser;


		EventAggregator.Sub<DeleteUserProfileEvent, UserProfileEventArgs>(OnDeleteUserProfileEvent);

		EventAggregator.Sub<AfterCreateOrRemoveFolderEvent>(OnHandleUserEvent);

		_ = EventAggregator.GetEvent<SelectedChangeUserProfileEvent>()
				.Subscribe(OnSelectedChanged);

		_ = EventAggregator.GetEvent<SavedUserProfileFolderEvent>()
				.Subscribe((e) => OnPropertyChanged(nameof(Folder)));

		_ = EventAggregator.GetEvent<UpdateStaleDataEvent>()
			 .Subscribe(LoadAsync);
	}

	public override async Task InitAsync(object? param)
	{
		IsWaiting = true;

		await base.InitAsync(param);

		if (!Loaded) {
			LoadAsync();
		}

		await InitializeScripts();
		InintializeLastSelectedAutomation();

		OnHandleUserEvent();

		IsWaiting = false;
	}

	private async Task InitializeScripts()
	{
		void AddMappedScripts(IEnumerable<PlaywriteRunScriptOptions> scripts)
		{
			PlaywrightScripts.AddMapped(scripts, (Func<PlaywriteRunScriptOptions, AutomationScriptModel>)(b => {
				var viewModel = new AutomationScriptModel(b);
				viewModel.Parameters.AddRange((IEnumerable<PlaywrightDescriptionParam>)b.Description!.Parameters);
				return viewModel;
			}));
		}

		PlaywrightScripts.Clear();

		AddMappedScripts(_plawrightRepository.GetBundledScrits());

		var usd = lib.Common.IoC.GetValue<string>("UserScriptsDirectory");
		if (usd.Is() && Directory.Exists(usd)) {
			AddMappedScripts(await _plawrightRepository.GetUserScripts(usd));
		}

		OnPropertyChanged(nameof(SelectedBrowserItem));
	}

	private void InintializeLastSelectedAutomation()
	{
		var lastSelectedBrowserString = lib.Common.IoC.GetValue<string>("LastSelectedBrowser");

		SelectedBrowserItem = !lastSelectedBrowserString.Is() ||
				!Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum)
			? BrowserItems[0]
			: BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);

		SelectedPlaywrightScript = PlaywrightScripts.FirstOrDefault(s => s.Title == lib.Common.IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];
	}

	private void OnHandleUserEvent()
	{
		OnPropertyChanged(nameof(ViewModels));
		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(HasProfileWithoutFolder));
		OnPropertyChanged(nameof(IsAddProfilesToFolderCommandEnabled));
	}

	private void OnViewModelChange(object sender, EventArgs args)
	{
		var items = ViewModels.Filter == null ? _mapping.ToList() : _mapping.Where(ViewModels.Filter).ToList();
		int count = items.Count;

		PaginatorViewModel.TotalCount = count;
		TotalCount = count;
	}

	public void Open(IUserProfileFolder? folder)
	{
		Folder = folder;

		UnselectItems();
		OnHandleUserEvent();
	}


	private void OnSelectedChanged(SelectedUserProfileEventArgs arr = null)
	{
		_selectedProfiles = _mapping.Where(profile => profile.IsSelected);
		SelectedCount = _selectedProfiles.Count();

		RunAutomationCommand.NotifyCanExecuteChanged();
	}


	[RelayCommand]
	private void SelectAll()
	{
		if (_mapping == null) {
			return;
		}

		foreach (var profile in ViewModels) {
			profile.IsSelected = true;
		}

		SelectedCount = ViewModels.Count;
	}

	[RelayCommand]
	private void SelectAllProfilesFromFolder()
	{
		if (_mapping == null) {
			return;
		}

		var profiles = _mapping
				.Where(p => p.UserProfile.FolderId == Folder?.Id || Folder?.Id == 0)
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
		if (await Mbox.Show("Delete User Profiles",
				$"Are you sure you want to delete {SelectedCount} profiles?",
				MBoxButtons.OkCancel,
				"DeleteLines")) {
			var profiles = _selectedProfiles.ToList();

			foreach (var profile in profiles) {
				await Task.Run(() => _userProfileService.Delete(profile.UserProfile));
				profile.IsSelected = false;
				_mapping.Remove(profile);
			}
			_viewModels = null;
			OnViewModelChange(this, EventArgs.Empty);
			ChangeProfilesInFavoriteFolder();
			OnSelectedChanged();
			OnHandleUserEvent();
		}
	}

	private void OnDeleteUserProfileEvent(UserProfileEventArgs obj)
	{
		var profile = _mapping.FirstOrDefault(profile => profile.UserProfile.Id == obj.UserProfile.Id);
		if (profile != null)
			profile.IsSelected = false;

		_mapping.Remove(profile);
		_viewModels = null;
		OnPropertyChanged(nameof(ViewModels));
		OnSelectedChanged();
		OnHandleUserEvent();
	}

	[RelayCommand]
	private void RemoveProfilesFromFolder()
	{
		if (_folder?.Id == 0 ||
				_selectedProfiles == null ||
				!_selectedProfiles.Any()) {
			return;
		}

		var ids = _selectedProfiles
				.Select(a => a.UserProfile.Id)
				.ToList();

		_userProfileService.MoveUserProfileToFolder(ids, null);
		Filter = p => p.FolderId == _folder?.Id;
		OnHandleUserEvent();
		ChangeProfilesInFavoriteFolder();
	}

	[RelayCommand]
	private async Task AddProfilesToFolder()
	{
		if (_folder?.Id == 0)
			return;

		if (await ContentDialogService.ShowAsync<IAddUserProfilesPopupView, IAddUserProfilesPopupViewModel>(
				viewModel => {
					viewModel.Title = "Add Profiles";
					viewModel.Folder = _folder;
				}) == IContentDialogResult.Primary) {
			Filter = p => p.FolderId == _folder.Id;
			OnHandleUserEvent();
		}
	}

	[RelayCommand]
	private async Task MoveProfilesToFolder()
	{
		var selectedUserProfiles = _selectedProfiles
				.Select(p => (IUserProfile)p.UserProfile)
				.ToList();

		if (await ContentDialogService.ShowAsync<IMoveUserProfilesPopupView, IMoveUserProfilesPopupViewModel>(
				 viewModel => {
					 viewModel.Title = "Add To Folder";
					 viewModel.Profiles = selectedUserProfiles;
				 }) == IContentDialogResult.Primary) {
			if (_folder?.Id != 0) {
				Filter = p => p.FolderId == _folder?.Id;
				OnHandleUserEvent();
			}
		}
	}

	private void ChangeProfilesInFavoriteFolder()
	{
		var folderId = Folder?.Id;
		if (folderId == null) return;

		EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Publish(new ChangeProfilesInFavoriteFolderEventArgs((int)folderId));
	}

	public async Task<IUserProfile> CreateNewProfile()
	{
		var folderId = HasFolder ? Folder?.Id : null;

		var profile = await _userProfileService.CreateAsync(folderId);
		if (ViewModels.Count == 1 || folderId != 0 && !ViewModels.Any(p => p.UserProfile.Id == profile.Id))
			OnHandleUserEvent();

		return profile;
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
		if (!ViewModels.Any(p => p.IsSelected == true)) {
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
		_cts.Cancel();
	}

	private void SetViewModelsFilter()
	{
		if (_viewModels == null) {
			return;
		}

		if (_filter == null) {
			_viewModels.Filter = null;
		} else {
			_viewModels.Filter = (viewModel) => _filter(viewModel.UserProfile);
		}

		OnViewModelChange(this, EventArgs.Empty);
	}

	private void LoadAsync()
	{
		ViewModels?.Clear();
		_viewModels = null;

		var userProfiles = _userProfileService.GetAll();

		_mapping = new ObservableCollection<IUserProfile, UserProfileViewModel>(
		userProfiles, profile => new UserProfileViewModel
				(_userProfileService, profile as UserProfile, _currentUser));

		_mapping.CollectionChanged += OnViewModelChange;

		OnHandleUserEvent();
	}

	private void ApplySearchFilter()
	{
		var searchText = SearchText?.ToLower();
		var hasSearchText = !string.IsNullOrWhiteSpace(SearchText);
		var isInCurrentFolder = Folder?.CreatorUserId != null;

		Filter = profile => FilterByFolder(profile, hasSearchText, isInCurrentFolder, searchText);

		OnPropertyChanged(nameof(ViewModels));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasNoItems));
	}

	private bool FilterByFolder(IUserProfile profile, bool hasSearchText, bool isInCurrentFolder, string searchText)
	{
		if (!hasSearchText && isInCurrentFolder) {
			return profile.FolderId == Folder.Id;
		}
		if (hasSearchText && isInCurrentFolder) {
			return profile.FolderId == Folder.Id && FilterByUserProfile(profile, searchText);
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

	public async void OnFilterTo(IUserProfile p = null)
	{
		while (!Loaded)
			await Task.Delay(250);

		if (p != null) {
			if (p.FolderId is int fid && fid != 0)
				ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().SetSelectedById(fid);
			else
				await ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);

			Filter = profile => p.Id == profile.Id;
		} else {
			//Filter = profile => 0 == profile.Id;
			ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().SetSelectedById(0);
			Filter = null;
			//ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>().OnNavigatingTo(null);
		}

		OnHandleUserEvent();
	}
}