using Chameleon.Controls.AssistantUsers.Interfaces;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using Chameleon.Interfaces.App.ShareFolders;
using Chameleon.Interfaces.Auth;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.Interfaces.Assistants;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.Input;
using Chameleon.Authorization;
using Chameleon.Interfaces.App.Users.AssistantUser.Events;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.Interfaces.App.Synchronization.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Microsoft.VisualBasic;
using DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using AutoMapper;
using Chameleon.lib.Common.Util;
using Chameleon.Common.Helpers;
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.ViewModels;

public partial class AssistantFolderPermissionViewModel(AssisShareFolderPermission shareFolderPermission, AssisShareFolderDto shareFolder) : ViewModelObjectBase {
	public AssisShareFolderPermission ShareFolderPermission { get; } = shareFolderPermission;
	public AssisShareFolderDto ShareFolder { get; } = shareFolder;
	public string? PermissionName => ShareFolderPermission.PermissionName;
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
	private async Task UpdatePermission()
	{
		try {
			if (IsGranted) {
				await ShareFoldersRepo.AddPermission(ShareFolder.id, ShareFolderPermission.PermissionId);
			} else {
				await ShareFoldersRepo.DeletePermission(ShareFolder.id, ShareFolderPermission.PermissionId);
			}

			Toaster.ShowSuccess($"{ShareFolderPermission.DisplayName} was updated successfully");
		} catch {
			IsGranted = !IsGranted;

			Toaster.ShowErr($"{ShareFolderPermission.DisplayName} update failed. Please try again.");
		}
	}
}
public partial class AssistantProfilePermissionViewModel(AssisProfilePermissionDto assistantProfilePermission) : ViewModelObjectBase {
	public AssisProfilePermissionDto AssistantProfilePermission { get; } = assistantProfilePermission;
	public string? PermissionName => AssistantProfilePermission.PermissionName;
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
	private async Task UpdatePermission()
	{
		try {
			_ = await UserAssistantRepo.UpdateProfilePermission(AssistantProfilePermission);

			Toaster.ShowSuccess($"{AssistantProfilePermission.DisplayName} was updated successfully");
		} catch {
			IsGranted = !IsGranted;
			Toaster.ShowErr($"{AssistantProfilePermission.DisplayName} update failed. Please try again.");
		}
	}
}

public partial class AssistantViewModelBase : ViewModelObjectBase {
	public virtual string? Name => "AssistantViewModelBase";
	[RelayCommand]
	public virtual void Unshare() { }
}
public partial class AssistantUserFolderViewModel	: AssistantViewModelBase {
	public event Action<AssisShareFolderDto>? OnUnshareFolder;
	public AssisShareFolderDto ShareFolder { get; }
	public ObservableCollection<AssistantFolderPermissionViewModel> PermissionViewModels { get; } = [];
	public AssistantUserFolderViewModel(AssisShareFolderDto shareFolder)
	{
		ShareFolder = shareFolder;
		foreach (var item in ShareFolder.FolderPermissions) {
			PermissionViewModels.Add(new(item, ShareFolder));
		}
	}

	public override string? Name => ShareFolder.FolderName;

	public AssistantFolderPermissionViewModel Outreach =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Outreach);

	public AssistantFolderPermissionViewModel Prospector =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Prospector);

	public AssistantFolderPermissionViewModel YouTube =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_YouTube);

	public AssistantFolderPermissionViewModel RSS =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_RSS);

	public AssistantFolderPermissionViewModel Curate =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Curate);

	public override void Unshare()
	{
		base.Unshare();
		OnUnshareFolder?.Invoke(ShareFolder);
	}
}
public partial class AssistantUserProfileViewModel
				: AssistantViewModelBase {
	public event Action<AssisProfileDto>? OnUnshare;

	private ObservableCollection<AssistantProfilePermissionViewModel> PermissionViewModels { get; } = [];
	public AssisProfileDto AssistantProfile { get; }

	public override string? Name => AssistantProfile.ProfileName;

	public AssistantUserProfileViewModel(AssisProfileDto assistantProfile)
	{
		AssistantProfile = assistantProfile;
	}

	public AssistantProfilePermissionViewModel Outreach =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Outreach);

	public AssistantProfilePermissionViewModel Prospector =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Prospector);

	public AssistantProfilePermissionViewModel YouTube =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_YouTube);

	public AssistantProfilePermissionViewModel RSS =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_RSS);

	public AssistantProfilePermissionViewModel Curate =>
			PermissionViewModels
					.Single(a => a.PermissionName == Consts.Permissions.Pages_Curate);

	public override void Unshare()
	{
		OnUnshare?.Invoke(AssistantProfile);

		base.Unshare();
	}

	public async Task InitPermissions()
	{
		var permissions = await UserAssistantRepo.GetAllProfilePermissions(AssistantProfile.id, AssistantProfile.ProfileId);
		PermissionViewModels.AddRange(permissions.Select(p=>new AssistantProfilePermissionViewModel(p)));

		OnPropertyChanged(nameof(PermissionViewModels));
		OnPropertyChanged(nameof(Outreach));
		OnPropertyChanged(nameof(Prospector));
		OnPropertyChanged(nameof(YouTube));
		OnPropertyChanged(nameof(RSS));
		OnPropertyChanged(nameof(Curate));
	}
}

public partial class AssistantUserViewModel : ViewModelObjectBase {
	private const string _unshareProfileDialogTitle = "Unshare Profile";
	private const string _unshareFolderDialogTitle = "Unshare Folder";
	private const string _deleteUserDialogTitle = "Delete User";


	public ObservableCollection<AssistantUserProfileViewModel> ProfileViewModels { get; } = [];
	public ObservableCollection<AssistantUserFolderViewModel> FolderViewModels { get; } = [];

	[ObservableProperty]
	private bool canCreateProfiles;

	public AssistDto UserAssistant { get; }

	public AssistantUserViewModel(AssistDto userAssistant)
	{
		UserAssistant = userAssistant;
		CanCreateProfiles = userAssistant.CanCreateProfiles;
	}
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

	public int Id {
		get => UserAssistant.id;
		set => UserAssistant.id = value;
	}
	public string? Username {
		get => UserAssistant.UserName;
		set => UserAssistant.UserName = value;
	}
	public string? Email {
		get => UserAssistant.EmailAddress;
		set => UserAssistant.EmailAddress = value;
	}

	#endregion

	#region Methods

	private async Task InitProfiles()
	{
		var profiles = await UserAssistantRepo.GetAllAssistantProfilesById(UserAssistant.id);
		ProfileViewModels
			.AddRange(profiles.Select(p=>new AssistantUserProfileViewModel(p)));
	}
	private async Task InitFolders()
	{
		var folders = await ShareFoldersRepo.GetAll(UserAssistant.id);
		FolderViewModels
				.AddRange(folders.Select(p => new AssistantUserFolderViewModel(p)));
	}

	[RelayCommand]
	private async Task DeleteAssistant()
	{
		if (await Mbox.Show(_deleteUserDialogTitle, $"Are you sure you want to delete {Username}", fontIconInfo: "Delete"))
			try {
				await UserAssistantRepo.Instance.Delete(UserAssistant.id);
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
							viewModel.AssistantId = UserAssistant.id;
						});
	}

	[RelayCommand]
	private async Task SendLicenceKey()
	{
		await CopyPasta.Copy($"{UserAssistant.EmailAddress} {UserAssistant.UserName} {UserAssistant.Password}");
	}
	private async Task AddFolders(long assistantId, IList<int> folderIds, IList<int> folderpermissionIds)
	{
		try {
			var newFolders = await ShareFoldersRepo.Share(assistantId, folderIds, folderpermissionIds);

			foreach (var newFolder in newFolders) {
				FolderViewModels.Add(new AssistantUserFolderViewModel(newFolder));
			}

			Toaster.ShowSuccess($"{folderIds.Count} folder(s) shared successfully");
		} catch {
			Toaster.ShowErr($"Failed to share folder(s). Please try again.");
		}
	}
	private async Task AddProfiles(long assistantId, IList<int> profileIds, IList<int> profilePermissions)
	{
		try {
			var profs = await UserAssistantRepo.AddProfiles(assistantId, profileIds, profilePermissions);

			ProfileViewModels.AddRange(profs.Select(p=>new AssistantUserProfileViewModel(p)));

			Toaster.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
		} catch {
			Toaster.ShowErr($"Failed to share profile(s). Please try again.");
		}
	}
	private void OnAddProfiles(AddProfilesEventArgs args)
	{
		if (UserAssistant.id != args.AssistantId) {
			return;
		}

		AddProfiles(args.AssistantId, args.Profiles.Select(i=>i.Id).ToList(), args.ProfilePermissionIds);
		AddFolders(args.AssistantId, args.Folders.Select(f=>f.Id).ToList(), args.FolderPermissionIds);
	}
	private async void OnUnshareProfile(UnshareProfileEventArgs args)
	{
		if (UserAssistant.id != args.AssistantProfile.Id) {
			return;
		}

		if (await Mbox.Show(_unshareProfileDialogTitle, $"Are you sure you want to unshare {args.AssistantProfile.ProfileName}? This will not affect other profiles."))
			try {
				await UserAssistantRepo.DeleteAssistantProfile(UserAssistant.id, args.AssistantProfile.ProfileId);

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
		if (UserAssistant.id != args.AssistantFolder.UserId) {
			return;
		}

		if (await Mbox.Show(_unshareFolderDialogTitle, $"Are you sure you want to unshare {args.AssistantFolder.FolderName}? This will not affect other folders."))
			try {
				ShareFoldersRepo.Instance.Delete(args.AssistantFolder.Id);

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
			UserAssistantRepo.SetCanCreateProfiles(Id, CanCreateProfiles);

			Toaster.ShowSuccess($"Permission to create profiles was successfully {(CanCreateProfiles ? "given" : "taken")}");
		} catch {
			Toaster.ShowErr($"Create profiles permission update failed. Please try again.");
		}
	}

	#endregion
}

public partial class AssistantUsersViewModel
			 : ViewModelObjectBase {
	private readonly IAuthSession _authSession = ContainerServiceHelper.Resolve<IAuthSession>()!;
	private readonly ISubject<IPageRequest> pageRequests = new BehaviorSubject<IPageRequest>(new PageRequest(0, 25));
	private readonly ReadOnlyObservableCollection<AssistantUserViewModel> assistants;
	public ReadOnlyObservableCollection<AssistantUserViewModel> ViewModels => assistants;

	public AssistantUsersViewModel(
			): base("User Management")
	{
		_ = UserAssistantRepo.Instance.ObservableCache
			.Connect()
			.Transform(p=> new AssistantUserViewModel(p))
			.Bind(out assistants)
			.Subscribe((i) => {
				if (ViewModels != null) {
					PaginatorViewModel ??= new PaginatorViewModel(ViewModels.Count);
					PaginatorViewModel.TotalCount = ViewModels.Count;
					TotalCount = PaginatorViewModel.TotalCount;
				}
			});

		EventAggregator.Sub<InviteUserAssistantEvent, InviteUserAssistantEventArgs>(OnCreate);
		EventAggregator.Sub<SavedUserAssistantEvent, SavedUserAssistantEventArgs>(OnUserAssistantSaved);
		EventAggregator.Sub<DeletedUserAssistantEvent, DeletedUserAssistantEventArgs>(OnUserAssistantDeleted);
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
			if (SetProperty(ref _paginatorViewModel, value))
				_paginatorViewModel!.ChangePageIndex += (s, a) => { pageRequests.OnNext(new PageRequest(_paginatorViewModel.PageIndex, 25)); };
		}
	}

	[RelayCommand]
	private async Task CreateNewUserAssistant()
	{
		if (ViewModels.Count >= _authSession.Limits.MaxAssistantsCount) {
			ShowOutOfLimitPopup();
		} else {
			_ = await Mbox.ShowContentDialog<IInviteUserOrAddProfilesView, IInviteUserOrAddProfilesViewModel>(
								viewModel => {
									viewModel.Title = "Invite User";
									viewModel.TitleText = "Invite new user and customise their access";
									viewModel.ShowInviteinfo = true;
								});
		}
	}

	private async void ShowOutOfLimitPopup()
	{
		if (await Mbox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
			ProUtil.GoToUrlDefault(Consts.GlobalSettings.PricingUrl);
	}
	private void SendLicenceKey(string emailAddress, string password)
	{
		var url = $"mailto:{emailAddress}?subject=Chameleon invitation&body=You’ve been invited to Chameleon. Your credentials:%0DEmail: {emailAddress}%0DKey: {password}%0D";
		ProUtil.GoToUrlDefault(url);
	}
	private void OnUserAssistantDeleted(DeletedUserAssistantEventArgs args)
	{
		//var itemToRemove = ViewModels
		//		.First(v => v.Id == args.Id);
		//ViewModels.Remove(itemToRemove);

		//Toaster.ShowSuccess($"{itemToRemove.Username} was deleted successfully");
	}
	private async void OnUserAssistantSaved(SavedUserAssistantEventArgs args)
	{
		_ = await UserAssistantRepo.Instance.Put(new AssistDto {
			UserName = args.AssistantName,
			EmailAddress = args.AssistantEmail,
			ProfileIds = args.ProfileIds,
			ProfilePermissionIds = args.ProfilePermissionids,
			FolderIds = args.FolderIds
		});

		if (args.ProfileIds?.Count > 0) Toaster.ShowSuccess($"{args.ProfileIds.Count} profile(s) shared successfully");

		if (args.FolderIds?.Count > 0) Toaster.ShowSuccess($"{args.FolderIds.Count} folder(s) shared successfully");
	}
	private async void OnCreate(InviteUserAssistantEventArgs args)
	{
		try {
			_ = await UserAssistantRepo.Instance.Create(new AssistDto {
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
}
