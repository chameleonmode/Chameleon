using Chameleon.app.Avalonia.Models;
using Chameleon.Avalonia.Common.Collections;
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
using Chameleon.app.Avalonia.Controls;
using FluentAvalonia.Core;
using System.Reactive.Linq;
using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;

public partial class UserProfilesViewModel : ViewModelObjectBase {
	private readonly IAuthSession _authSession = IoC.GetService<IAuthSession>()!;

	private readonly ISysBrowserService _systemBrowserManager = IoC.GetService<ISysBrowserService>()!;
	private readonly IPlaywrightScriptRepository _plawrightRepository = IoC.GetService<IPlaywrightScriptRepository>()!;
	private readonly IPlaywriteService _playwriteService = IoC.GetService<IPlaywriteService>()!;

	private PaginatorViewModel? _paginatorViewModel;
	public AvList<PlaywrightScript> PlaywrightScripts { get; } = [];

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
	private PlaywrightScript? selectedPlaywrightScript;
	[ObservableProperty]
	private string searchText = string.Empty;
	[ObservableProperty]
	private UPFolderDto? folder;
	[ObservableProperty]
	private int selectedCount;

	private CancellationTokenSource? _cts;
	private IEnumerable<ObsProfile>? _selectedProfiles;

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
	public PaginatorViewModel? PaginatorViewModel {
		get => _paginatorViewModel;
		set {
			if (SetProperty(ref _paginatorViewModel, value)) {
				_paginatorViewModel!.ChangePageIndex += (s, a) => { pageRequests.OnNext(new PageRequest(_paginatorViewModel.PageIndex, Consts.PageinationPageItems)); };
			}
		}
	}
	
	private List<ObsProfile> GetSelectedProfiles => _selectedProfiles!.ToList();
	public bool HasSelectedItems => Profiles.Any(v => v.IsSelected);
	public bool IsProfilesExist => UserProfileFoldersViewModel.Instance.AllProfiles?.IsFolderNotEmpty == false;
	public bool HasNoItems => Profiles.Count == 0;
	public bool HasProfileWithoutFolder => Profiles != null && Profiles.Any(profile => profile.Dto?.folderId != null);
	public string SelectedFolderTitle => Folder?.title ?? "All profiles";
	//
	public Func<ObsProfile, bool> FilterPredicate => p => Folder == null || Folder.id == 0 || (Folder != null && Folder.id != 0 && p.Dto?.folderId == Folder?.id);

	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.ObsProfileCompares.AscendingComparer);
	private readonly BehaviorSubject<IPageRequest> pageRequests = new(new PageRequest(0, Consts.PageinationPageItems));
	private readonly BehaviorSubject<Func<ObsProfile, bool>> filter;

	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	private readonly ReadOnlyObservableCollection<ObsProfile> profiles;
	public ReadOnlyObservableCollection<ObsProfile> Profiles => profiles;

	private UserProfilesViewModel()
	{
		//Func<ObsProfile, IObservable<bool>> filterFactory = p => Observable.Return(predicate(p));
		filter = new BehaviorSubject<Func<ObsProfile, bool>>(FilterPredicate);
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i, onSelectedChanged: OnSelectedChanged))
			.Filter(filter)
			//.FilterOnObservable(filterFactory)
			.SortAndPage(Compares.ObsProfileCompares.AscendingComparer, pageRequests)
			.SortAndBind(out profiles, profilesCompareObservable)
			.Subscribe((i) => {
				if (Profiles != null) {
					PaginatorViewModel ??= new PaginatorViewModel(Profiles.Count);
					PaginatorViewModel.TotalCount = Profiles.Count;
					TotalCount = PaginatorViewModel.TotalCount;
				}
				OnHandleUserEvent();
				//foreach (var update in i) {
				//	var curIndex = update.CurrentIndex == -1 ? 0 : update.CurrentIndex;
				//	var prevIndex = update.PreviousIndex == -1 ? 0 : update.PreviousIndex;
				//	switch (update.Reason) {
				//		case ChangeReason.Add:
				//			Folders.Add(update.Current);
				//			break;
				//		case ChangeReason.Remove:
				//			_ = Folders.Remove(update.Current);
				//			break;
				//		case ChangeReason.Moved:
				//			Folders.Move(prevIndex, curIndex);
				//			break;
				//		case ChangeReason.Update:
				//			//var indx = Folders.IndexOf(update.Current);
				//			//Folders.RemoveAt(prevIndex);
				//			//Folders.Insert(curIndex, update.Current);
				//			break;
				//	}
				//}
			});
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		//await InitializeScripts();
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

		//InintializeLastSelectedAutomation();
		var lastSelectedBrowserString = IoC.GetValue<string>("LastSelectedBrowser");
		SelectedBrowserItem = !lastSelectedBrowserString.Is() ||
				!Enum.TryParse(typeof(SystemBrowserType), lastSelectedBrowserString, out var browserEnum)
			? BrowserItems[0]
			: BrowserItems.First(b => b.SystemBrowserType == (SystemBrowserType)browserEnum);
		SelectedPlaywrightScript = PlaywrightScripts.FirstOrDefault(s => s.Title == IoC.GetValue<string>("LastRunScriptId")) ?? PlaywrightScripts[0];

		OnPropertyChanged(nameof(SelectedBrowserItem));
		OnHandleUserEvent();
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		profilesCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsProfileCompares.DescendingComparer,
			_ => Compares.ObsProfileCompares.AscendingComparer
		});
	}
	partial void OnSelectedBrowserItemChanged(SystemBrovserItem? value)
	{
		if (value == null)
			return;

		var cur = IoC.GetValue<string>("LastSelectedBrowser");
		if (cur != value.SystemBrowserType.ToString())
			IoC.SetValue(value.SystemBrowserType.ToString(), "LastSelectedBrowser");
	}
	partial void OnSelectedPlaywrightScriptChanged(PlaywrightScript? value)
	{
		var cur = IoC.GetValue<string>("LastRunScriptId");
		if (value!= null && cur != value.Title)
			IoC.SetValue(value.Title, "LastRunScriptId");
	}
	partial void OnSearchTextChanged(string value)
	{
		if (value.Is())
			filter.OnNext(p => p.Title?.Contains(value, StringComparison.CurrentCultureIgnoreCase) == true);
		else
			filter.OnNext(FilterPredicate);

		OnHandleUserEvent();
	}
	partial void OnFolderChanged(UPFolderDto? value)
	{
		SearchText = string.Empty;
		HasFolder = value?.id != default && value?.id != 0;
		OnPropertyChanged(nameof(SelectedFolderTitle));
		SetViewModelsFilter();
	}
	partial void OnSelectedCountChanged(int value)
	{
		OnPropertyChanged(nameof(HasSelectedItems));
	}

	public void Open(UPFolderDto? folder)
	{
		Folder = folder;

		UnselectItems();
		OnHandleUserEvent();
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
	public async void OnFilterTo(ObsProfile? p = null)
	{
		_ = await LoadedTCS.Task;

		if (p != null) {
			if (p.Dto?.folderId is int fid && fid != 0)
				UserProfileFoldersViewModel.Instance.SetSelectedById(fid);
			else
				await UserProfileFoldersViewModel.Instance.OnNavigatingTo(null);
		} else {
			UserProfileFoldersViewModel.Instance.SetSelectedById(0);
		}

		SetViewModelsFilter();
	}

	private void OnHandleUserEvent()
	{
		OnPropertyChanged(nameof(PaginatorViewModel));
		OnPropertyChanged(nameof(HasNoItems));
		OnPropertyChanged(nameof(IsProfilesExist));
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(HasProfileWithoutFolder));
	}
	private void OnSelectedChanged()
	{
		_selectedProfiles = Profiles.Where(profile => profile.IsSelected);
		SelectedCount = _selectedProfiles.Count();
	}
	private void SetViewModelsFilter()
	{
		filter.OnNext(FilterPredicate);
		PaginatorViewModel = new PaginatorViewModel(Profiles.Count) {
			TotalCount = Profiles.Count
		};
		TotalCount = PaginatorViewModel.TotalCount;
		OnHandleUserEvent();
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
			OnSelectedChanged();
			SetViewModelsFilter();
		}
	}

	[RelayCommand]
	private async Task RemoveProfilesFromFolder()
	{
		if (Folder?.id == 0 ||
				_selectedProfiles == null ||
				!_selectedProfiles.Any()) {
			return;
		}

		var ids = _selectedProfiles
				.Select(a => a.Dto!.id)
				.ToList();

		var res = await UserProfilesRepo.MoveUserProfileToFolder(ids, null);
		if (res.success) {
			//Filter = p => p.FolderId == _folder?.id;
			SetViewModelsFilter();
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
		

		if (await Mbox.ShowTaskDialog<AddUserProfilesPupViewModel, AddUserProfilesPopupUserControl>(() => addvm, 
			header: addvm.Title, 
			subHeader: $"Select profiles you want to add to {Folder!.title} folder:", 
			symbas: Enums.Symbas.Folder, 
			btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
			var ids = addvm.SelectedViewModels?
				.Select(a => a.Dto!.id)
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
		addvm.Folders.AddRange(UserProfileFoldersViewModel.Instance.Folders);

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
			var browserWasNotOpened = profile.SBI![SelectedBrowserItem!.SystemBrowserType] == null;
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

	public static UserProfilesViewModel Instance { get; } = new UserProfilesViewModel();
}
