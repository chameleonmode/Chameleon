using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Core.Util;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Auth;
using Chameleon.Core.Settings;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.Interfaces.Assistants;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Core.Collections;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Authorization;
using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.Interfaces.App.Synchronization.Events;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.ViewModels;

public interface IUserAssistantService
		: Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency {
	ICollection<IUserAssistant> Get();
	Task<ICollection<IUserAssistant>> GetAsync();
	void Save(IUserAssistant userAssistant);
	void DeleteAssistant(IUserAssistant userAssistant);
	IList<IAssistantProfile> GetAllAssistantProfilesById(long assistantId);
	void DeleteAssistantProfile(IAssistantProfile assistantProfile);
	IList<IAssistantProfilePermission> GetAllProfilePermissions(long assistantId, int profileId);
	void UpdateProfilePermission(IAssistantProfilePermission assistantProfilePermission);
	void ShareUserProfile(int profileId, IList<long> assistantUserIds, IList<string> permissionNames);
	void AddProfiles(IUserAssistant userAssistant);
	void SetCanCreateProfiles(long assistantId, bool canCreateProfiles);
}
public class UserAssistant
		: IUserAssistant {
	public long Id { get; set; }
	public string UserName { get; set; }
	public string EmailAddress { get; set; }
	public string Password { get; set; }
	public bool CanCreateProfiles { get; set; }
	public IList<int> ProfileIds { get; set; }
	public IList<int> ProfilePermissionIds { get; set; }
	public IList<int> FolderIds { get; set; }
	public IList<int> FolderPermissionIds { get; set; }
}

public class AssistantProfile
		: IAssistantProfile {
	public long Id { get; set; }
	public int ProfileId { get; set; }
	public string ProfileName { get; set; }
}

public partial class AssistantFolderPermissionViewModel
			 : ViewModelObjectBase {
	private readonly IShareFoldersService _shareFoldersService;

	public AssistantFolderPermissionViewModel(
			IShareFoldersService shareFoldersService,
			IShareFolderPermission shareFolderPermission,
			IShareFolder shareFolder)
	{
		_shareFoldersService = shareFoldersService;

		ShareFolderPermission = shareFolderPermission;
		ShareFolder = shareFolder;
	}

	public IShareFolderPermission ShareFolderPermission { get; }
	public IShareFolder ShareFolder { get; }

	public string PermissionName => ShareFolderPermission.PermissionName;
	public bool IsGranted {
		get => ShareFolderPermission.IsGranted;
		set {
			if (ShareFolderPermission.IsGranted != value) {
				ShareFolderPermission.IsGranted = value;
				OnPropertyChanged(nameof(IsGranted));
			}
		}
	}

	[RelayCommand]
	private void UpdatePermission()
	{
		try {
			if (IsGranted) {
				_shareFoldersService.AddPermission(ShareFolder.Id, ShareFolderPermission.PermissionId);
			} else {
				_shareFoldersService.DeletePermission(ShareFolder.Id, ShareFolderPermission.PermissionId);
			}

			Toaster.ShowSuccess($"{ShareFolderPermission.DisplayName} was updated successfully");
		} catch {
			IsGranted = !IsGranted;

			Toaster.ShowErr($"{ShareFolderPermission.DisplayName} update failed. Please try again.");
		}
	}
}

public partial class AssistantProfilePermissionViewModel
				: ViewModelObjectBase {
	private readonly IUserAssistantService _userAssistantService;

	public AssistantProfilePermissionViewModel(
			IUserAssistantService userAssistantService,
			IAssistantProfilePermission assistantProfilePermission
			)
	{
		_userAssistantService = userAssistantService;

		AssistantProfilePermission = assistantProfilePermission;
	}

	public IAssistantProfilePermission AssistantProfilePermission { get; }
	public string PermissionName => AssistantProfilePermission.PermissionName;
	public bool IsGranted {
		get => AssistantProfilePermission.IsGranted;
		set {
			if (AssistantProfilePermission.IsGranted != value) {
				AssistantProfilePermission.IsGranted = value;
				OnPropertyChanged(nameof(IsGranted));
			}
		}
	}

	[RelayCommand]
	private void UpdatePermission()
	{
		try {
			_userAssistantService.UpdateProfilePermission(AssistantProfilePermission);

			Toaster.ShowSuccess($"{AssistantProfilePermission.DisplayName} was updated successfully");
		} catch {
			IsGranted = !IsGranted;

			Toaster.ShowErr($"{AssistantProfilePermission.DisplayName} update failed. Please try again.");
		}
	}
}
public partial class AssistantUserFolderViewModel
			: AssistantViewModelBase {
	private readonly IEventAggregator _eventAggregator;
	private readonly IShareFoldersService _shareFoldersService;

	private ObservableCollection<IShareFolderPermission, AssistantFolderPermissionViewModel> _permissionMapping;

	public AssistantUserFolderViewModel(
			IEventAggregator eventAggregator,
			IShareFoldersService shareFoldersService,
			IShareFolder shareFolder)
	{
		_eventAggregator = eventAggregator;
		_shareFoldersService = shareFoldersService;

		ShareFolder = shareFolder;

		InitPermissions();
	}

	public IShareFolder ShareFolder { get; }
	public override string Name => ShareFolder.FolderName;

	private ObservableCollectionView<AssistantFolderPermissionViewModel> _permissionViewModels;
	public ObservableCollectionView<AssistantFolderPermissionViewModel> PermissionViewModels {
		get {
			if (_permissionViewModels == null && _permissionMapping != null) {
				_permissionViewModels = new ObservableCollectionView<AssistantFolderPermissionViewModel>(_permissionMapping);
			}
			return _permissionViewModels;
		}
		set => SetProperty(ref _permissionViewModels, value);
	}

	public AssistantFolderPermissionViewModel Outreach =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Outreach);

	public AssistantFolderPermissionViewModel Prospector =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Prospector);

	public AssistantFolderPermissionViewModel YouTube =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_YouTube);

	public AssistantFolderPermissionViewModel RSS =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_RSS);

	public AssistantFolderPermissionViewModel Curate =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Curate);

	public override void Unshare()
	{
		_eventAggregator
				.GetEvent<UnshareFolderEvent>()
				.Publish(new UnshareFolderEventArgs(ShareFolder));

		base.Unshare();
	}

	private void InitPermissions()
	{
		_permissionMapping = new ObservableCollection<IShareFolderPermission, AssistantFolderPermissionViewModel>(
				ShareFolder.FolderPermissions, folderPermission => new AssistantFolderPermissionViewModel(
						_shareFoldersService,
						folderPermission,
						ShareFolder)
				);
	}
}

public partial class AssistantUserProfileViewModel
				: AssistantViewModelBase {
	private readonly IEventAggregator _eventAggregator;
	private readonly IUserAssistantService _userAssistantService;
	private readonly IUserProfileService _userProfileService;

	private ObservableCollection<IAssistantProfilePermission, AssistantProfilePermissionViewModel> _permissionMapping;

	public AssistantUserProfileViewModel(
			IEventAggregator eventAggregator,
			IUserAssistantService userAssistantService,
			IUserProfileService userProfileService,
			IAssistantProfile assistantProfile
			)
	{
		_eventAggregator = eventAggregator;
		_userAssistantService = userAssistantService;
		_userProfileService = userProfileService;

		AssistantProfile = assistantProfile;

		InitPermissions();

		_eventAggregator
				.GetEvent<UpdateStaleDataEvent>()
				.Subscribe(RefreshTitle);
		_eventAggregator
				.GetEvent<SavedUserProfileEvent>()
				.Subscribe(args => RefreshTitle());
	}

	private void RefreshTitle()
	{
		var profile = _userProfileService.Get(AssistantProfile.ProfileId);
		AssistantProfile.ProfileName = profile.Title;
		OnPropertyChanged(nameof(Name));
	}

	public IAssistantProfile AssistantProfile { get; }
	public override string Name => AssistantProfile.ProfileName;

	private ObservableCollectionView<AssistantProfilePermissionViewModel> _permissionViewModels;
	public ObservableCollectionView<AssistantProfilePermissionViewModel> PermissionViewModels {
		get {
			if (_permissionViewModels == null && _permissionMapping != null) {
				_permissionViewModels = new ObservableCollectionView<AssistantProfilePermissionViewModel>(_permissionMapping);
			}
			return _permissionViewModels;
		}
		set => SetProperty(ref _permissionViewModels, value);
	}

	public AssistantProfilePermissionViewModel Outreach =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Outreach);

	public AssistantProfilePermissionViewModel Prospector =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Prospector);

	public AssistantProfilePermissionViewModel YouTube =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_YouTube);

	public AssistantProfilePermissionViewModel RSS =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_RSS);

	public AssistantProfilePermissionViewModel Curate =>
			PermissionViewModels
					.Single(a => a.PermissionName == PermissionNames.Pages_Curate);


	public override void Unshare()
	{
		_eventAggregator
				.GetEvent<UnshareProfileEvent>()
				.Publish(new UnshareProfileEventArgs(AssistantProfile));

		base.Unshare();
	}

	private void InitPermissions()
	{
		var permissions = _userAssistantService.GetAllProfilePermissions(AssistantProfile.Id, AssistantProfile.ProfileId);

		_permissionMapping = new ObservableCollection<IAssistantProfilePermission, AssistantProfilePermissionViewModel>(
				permissions, permission => new AssistantProfilePermissionViewModel(
						_userAssistantService,
						permission)
				);

		OnPropertyChanged(nameof(PermissionViewModels));
		OnPropertyChanged(nameof(Outreach));
		OnPropertyChanged(nameof(Prospector));
		OnPropertyChanged(nameof(YouTube));
		OnPropertyChanged(nameof(RSS));
		OnPropertyChanged(nameof(Curate));
	}
}

public partial class AssistantUserViewModel(
				IUserAssistant userAssistant,
				IUserAssistantService userAssistantService,
				IShareFoldersService shareFoldersService,
				IUserProfileService userProfileService)
			 : ViewModelObjectBase {
	private const string _unshareProfileDialogTitle = "Unshare Profile";
	private const string _unshareFolderDialogTitle = "Unshare Folder";
	private const string _deleteUserDialogTitle = "Delete User";

	private ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel> _profileMapping;
	private ObservableCollection<IShareFolder, AssistantUserFolderViewModel> _folderMapping;

	[ObservableProperty]
	private bool canCreateProfiles = userAssistant.CanCreateProfiles;

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (!Loaded) {
			await InitProfiles();
			await InitFolders();

			EventAggregator
					.GetEvent<UnshareProfileEvent>()
					.Subscribe(OnUnshareProfile);

			EventAggregator
					.GetEvent<AddProfilesEvent>()
					.Subscribe(OnAddProfiles);

			EventAggregator
					.GetEvent<UnshareFolderEvent>()
					.Subscribe(OnUnshareFolder);
		}
	}

	#region Properties
	public IUserAssistant UserAssistant => userAssistant;

	public long Id {
		get => UserAssistant.Id;
		set => UserAssistant.Id = value;
	}
	public string Username {
		get => UserAssistant.UserName;
		set => UserAssistant.UserName = value;
	}
	public string Email {
		get => UserAssistant.EmailAddress;
		set => UserAssistant.EmailAddress = value;
	}

	private ObservableCollectionView<AssistantUserProfileViewModel> _profileViewModels;
	public ObservableCollectionView<AssistantUserProfileViewModel> ProfileViewModels {
		get {
			if (_profileViewModels == null && _profileMapping != null) {
				_profileViewModels = new ObservableCollectionView<AssistantUserProfileViewModel>(_profileMapping);
			}
			return _profileViewModels;
		}
	}

	private ObservableCollectionView<AssistantUserFolderViewModel> _folderViewModels;
	public ObservableCollectionView<AssistantUserFolderViewModel> FolderViewModels {
		get {
			if (_folderViewModels == null && _folderMapping != null) {
				_folderViewModels = new ObservableCollectionView<AssistantUserFolderViewModel>(_folderMapping);
			}
			return _folderViewModels;
		}
	}

	#endregion


	#region Methods

	private async Task InitProfiles()
	{
		var profiles = await Task.Run(() => userAssistantService.GetAllAssistantProfilesById(UserAssistant.Id));

		_profileMapping = new ObservableCollection<IAssistantProfile, AssistantUserProfileViewModel>(
				profiles, profile => new AssistantUserProfileViewModel(
						EventAggregator,
						userAssistantService,
						userProfileService,
						profile));
	}
	private async Task InitFolders()
	{
		var folders = await Task.Run(() => shareFoldersService.GetAll(UserAssistant.Id));

		_folderMapping = new ObservableCollection<IShareFolder, AssistantUserFolderViewModel>(
				folders, folder => new AssistantUserFolderViewModel(
						EventAggregator,
						shareFoldersService,
						folder));
	}

	[RelayCommand]
	private async Task DeleteAssistant()
	{
		if (await Mbox.Show(_deleteUserDialogTitle, $"Are you sure you want to delete {Username}", fontIconInfo: "Delete"))
			try {
				userAssistantService.DeleteAssistant(UserAssistant);
			} catch {
				Toaster.ShowErr($"Failed to delete {UserAssistant.UserName}. Please try again.");
			}
	}

	[RelayCommand]
	private void AddMoreProfiles()
	{
		Mbox.ShowContentDialog<IInviteUserOrAddProfilesView, IInviteUserOrAddProfilesViewModel>(
						viewModel => {
							viewModel.Title = "Add Profiles";
							viewModel.TitleText = "Add access to specific Profiles or whole Folders for this user";
							viewModel.ShowInviteinfo = false;
							viewModel.AssistantId = UserAssistant.Id;
						});
	}

	[RelayCommand]
	private async Task SendLicenceKey()
	{
		await CopyPasta.Copy($"{UserAssistant.EmailAddress} {UserAssistant.UserName} {UserAssistant.Password}");
	}
	private void AddFolders(AddProfilesEventArgs args)
	{
		var folderIds = args.Folders?
				.Select(a => a.Id)
				.ToList();

		if (folderIds == null || folderIds?.Count == 0) {
			return;
		}

		try {
			var newFolders = shareFoldersService.Share(args.AssistantId, folderIds, args.FolderPermissionIds);

			foreach (var newFolder in newFolders) {
				var newVm = new AssistantUserFolderViewModel(
						EventAggregator,
						shareFoldersService,
						newFolder
						);

				FolderViewModels.Add(newVm);
			}

			Toaster.ShowSuccess($"{folderIds.Count} folder(s) shared successfully");
		} catch {
			Toaster.ShowErr($"Failed to share folder(s). Please try again.");
		}
	}
	private void AddProfiles(AddProfilesEventArgs args)
	{
		var folderIds = args.Folders?
				.Select(a => a.Id)
				.ToList();

		var profileIds = args.Profiles?
				.Select(a => a.Id)
				.ToList();

		if (profileIds == null || profileIds?.Count == 0) {
			return;
		}

		try {
			userAssistantService.AddProfiles(new UserAssistant {
				Id = args.AssistantId,
				ProfileIds = profileIds,
				ProfilePermissionIds = args.ProfilePermissionIds,
				FolderIds = folderIds,
				FolderPermissionIds = args.FolderPermissionIds
			});

			foreach (var profile in args.Profiles) {
				var assistantProfile = new AssistantProfile() {
					Id = args.AssistantId,
					ProfileId = profile.Id,
					ProfileName = profile.Title
				};

				var newProfile = new AssistantUserProfileViewModel(
						EventAggregator,
						userAssistantService,
						userProfileService,
						assistantProfile);

				ProfileViewModels.Add(newProfile);
			}

			Toaster.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
		} catch {
			Toaster.ShowErr($"Failed to share profile(s). Please try again.");
		}
	}
	private void OnAddProfiles(AddProfilesEventArgs args)
	{
		if (UserAssistant.Id != args.AssistantId) {
			return;
		}

		AddProfiles(args);
		AddFolders(args);
	}
	private async void OnUnshareProfile(UnshareProfileEventArgs args)
	{
		if (UserAssistant.Id != args.AssistantProfile.Id) {
			return;
		}

		if (await Mbox.Show(_unshareProfileDialogTitle, $"Are you sure you want to unshare {args.AssistantProfile.ProfileName}? This will not affect other profiles."))
			try {
				userAssistantService.DeleteAssistantProfile(args.AssistantProfile);

				var viewModelToDelete = ProfileViewModels
						.Single(v => v.AssistantProfile.ProfileId == args.AssistantProfile.ProfileId);

				ProfileViewModels.Remove(viewModelToDelete);

				Toaster.ShowSuccess($"{args.AssistantProfile.ProfileName} was unshared successfully");
			} catch {
				Toaster.ShowErr($"Failed to unshare profile. Please try again.");
			}
	}
	private async void OnUnshareFolder(UnshareFolderEventArgs args)
	{
		if (UserAssistant.Id != args.AssistantFolder.UserId) {
			return;
		}

		if (await Mbox.Show(_unshareFolderDialogTitle, $"Are you sure you want to unshare {args.AssistantFolder.FolderName}? This will not affect other folders."))
			try {
				shareFoldersService.Delete(args.AssistantFolder.Id);

				var viewModelToDelete = FolderViewModels
							 .Single(v => v.ShareFolder.FolderId == args.AssistantFolder.FolderId);

				FolderViewModels.Remove(viewModelToDelete);

				Toaster.ShowSuccess($"{args.AssistantFolder.FolderName} was unshared successfully");
			} catch {
				Toaster.ShowErr($"Failed to unshare folder. Please try again.");
			}
	}

	partial void OnCanCreateProfilesChanged(bool oldValue, bool newValue)
	{
		UserAssistant.CanCreateProfiles = newValue;
		SetCanCreateProfiles();
	}

	[RelayCommand]
	private void SetCanCreateProfiles()
	{
		try {
			userAssistantService.SetCanCreateProfiles(Id, CanCreateProfiles);

			Toaster.ShowSuccess($"Permission to create profiles was successfully {(CanCreateProfiles ? "given" : "taken")}");
		} catch {
			Toaster.ShowErr($"Create profiles permission update failed. Please try again.");
		}
	}

	#endregion
}

public partial class AssistantViewModelBase : ObservableObject {
	public virtual string Name => "AssistantViewModelBase";

	[RelayCommand]
	public virtual void Unshare()
	{
		OnPropertyChanged(string.Empty);
	}
}

public partial class AssistantUsersViewModel
			 : ViewModelObjectBase {
	private readonly IUserAssistantService _userAssistantService;
	private readonly IAuthSession _authSession;
	private readonly IShareFoldersService _shareFoldersService;
	private readonly IUserProfileService _userProfileService;

	private ObservableCollection<IUserAssistant, AssistantUserViewModel> _mapping;

	public AssistantUsersViewModel(
			IUserAssistantService userAssistantService,
			IAuthSession authSession,
			IShareFoldersService shareFoldersService,
			IUserProfileService userProfileService
			)
	{
		_userAssistantService = userAssistantService;
		//_unshareProfilePopupView = unshareProfilePopupView;
		//_dialogWindowsService = dialogWindowsService;
		// _inviteUserOrAddProfilesPopupService = inviteUserOrAddProfilesPopupService;
		// _deleteAssistantUserPopupView = deleteAssistantUserPopupView;
		_authSession = authSession;
		//_upgradePlanPopupView = upgradePlanPopupView;
		_shareFoldersService = shareFoldersService;
		_userProfileService = userProfileService;

		Title = "User Management";
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);

		if (Loaded)
			return;

		EventAggregator.Sub<InviteUserAssistantEvent, InviteUserAssistantEventArgs>(OnCreate);

		EventAggregator.Sub<SavedUserAssistantEvent, SavedUserAssistantEventArgs>(OnUserAssistantSaved);
		//.GetEvent<SavedUserAssistantEvent>()
		//.Subscribe(args => OnUserAssistantSaved(args));

		EventAggregator
				.GetEvent<DeletedUserAssistantEvent>()
				.Subscribe(OnUserAssistantDeleted);

		await InitUserAssistantsAsync();

		// OnPropertyChanged(string.Empty);
	}

	private int _totalCount;
	public int TotalCount {
		get => _totalCount;
		set => SetProperty(ref _totalCount, value);
	}

	private PaginatorViewModel _paginatorViewModel;
	public PaginatorViewModel PaginatorViewModel {
		get => _paginatorViewModel;
		set {
			if (SetProperty(ref _paginatorViewModel, value)) _paginatorViewModel.ChangePageIndex += OnChangePage;
		}
	}

	private ObservableCollectionView<AssistantUserViewModel> _viewModels;
	public ObservableCollectionView<AssistantUserViewModel> ViewModels {
		get {
			if (_viewModels == null && _mapping != null) {
				_viewModels = new ObservableCollectionView<AssistantUserViewModel>(_mapping);

				_mapping.CollectionChanged += OnViewModelChange;
				InitPaginator();
			}

			return _viewModels;
		}
	}

	[RelayCommand]
	private void CreateNewUserAssistant()
	{
		if (_viewModels?.Items.Count >= _authSession.Limits.MaxAssistantsCount) ShowOutOfLimitPopup();
		else {
			Mbox.ShowContentDialog<IInviteUserOrAddProfilesView, IInviteUserOrAddProfilesViewModel>(
								viewModel => {
									viewModel.Title = "Invite User";
									viewModel.TitleText = "Invite new user and customise their access";
									viewModel.ShowInviteinfo = true;
								});
		}
	}

	private async Task InitUserAssistantsAsync()
	{
		var assistants = await Task.Run(() => _userAssistantService.Get());

		_mapping = new ObservableCollection<IUserAssistant, AssistantUserViewModel>(
				assistants, userAssistant =>
				new AssistantUserViewModel
						(userAssistant,
						_userAssistantService,
						_shareFoldersService,
						_userProfileService));

		foreach (var a in _mapping)
			await a.InvokeInitializeAsyncCommand();

		OnPropertyChanged(nameof(ViewModels));
	}

	private void InitPaginator()
	{
		PaginatorViewModel = new PaginatorViewModel(_viewModels.Count);
		ViewModels.Offset = PaginatorViewModel.Skip;
		ViewModels.Limit = PaginatorViewModel.OnPageItems;
		TotalCount = PaginatorViewModel.TotalCount;
	}
	private async void ShowOutOfLimitPopup()
	{
		if (await Mbox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
			ProUtil.GoToUrlDefault(GlobalSettings.PricingUrl);
	}
	private void SendLicenceKey(string emailAddress, string password)
	{
		var url = $"mailto:{emailAddress}?subject=Chameleon invitation&body=You’ve been invited to Chameleon. Your credentials:%0DEmail: {emailAddress}%0DKey: {password}%0D";
		ProUtil.GoToUrlDefault(url);
	}
	private void OnUserAssistantDeleted(DeletedUserAssistantEventArgs args)
	{
		var itemToRemove = ViewModels
				.First(v => v.Id == args.Id);
		ViewModels.Remove(itemToRemove);

		Toaster.ShowSuccess($"{itemToRemove.Username} was deleted successfully");
	}
	private void OnUserAssistantSaved(SavedUserAssistantEventArgs args)
	{
		var userAssistant = new UserAssistant {
			Id = args.Id,
			UserName = args.AssistantName,
			EmailAddress = args.AssistantEmail,
			Password = args.Password,
			ProfileIds = args.ProfileIds,
			ProfilePermissionIds = args.ProfilePermissionids
		};

		var assistantUserViewModel = new AssistantUserViewModel(
				userAssistant,
				_userAssistantService,
				_shareFoldersService,
				_userProfileService);

		var isExist = ViewModels
				.Any(v => v.Id == userAssistant.Id);

		if (!isExist) {
			ViewModels.Add(assistantUserViewModel);
			SendLicenceKey(userAssistant.EmailAddress, userAssistant.Password);
		}

		if (args.ProfileIds?.Count > 0) Toaster.ShowSuccess($"{args.ProfileIds.Count} profile(s) shared successfully");

		if (args.FolderIds?.Count > 0) Toaster.ShowSuccess($"{args.FolderIds.Count} folder(s) shared successfully");
	}
	private void OnCreate(InviteUserAssistantEventArgs args)
	{
		try {
			_userAssistantService.Save(new UserAssistant {
				UserName = args.AssistantName,
				EmailAddress = args.AssistantEmail,
				ProfileIds = args.ProfileIds,
				ProfilePermissionIds = args.ProfilePermissionIds,
				FolderIds = args.FolderIds,
				FolderPermissionIds = args.FolderPermissionIds
			});
		} catch {
			Toaster.ShowErr($"Failed to invite the user. Please try again.");
		}
	}

	private void OnChangePage(object sender, EventArgs e)
	{
		ViewModels.Offset = PaginatorViewModel.Skip;
	}

	private void OnViewModelChange(object sender, EventArgs args)
	{
		var count = _viewModels.Items.Count;
		PaginatorViewModel.TotalCount = count;
		TotalCount = count;

		OnPropertyChanged(nameof(ViewModels));
	}
}
