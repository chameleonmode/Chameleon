using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Api;
using Chameleon.lib.Util;
using Chameleon.lib.Helpers;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.client.Features.Tenants.Members.Dialogs;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Dto;
using Chameleon.client.Features.Projects;
using Chameleon.lib.WebBrowser;

namespace Chameleon.client.Features.Tenants.Members;

public partial class AssistantUsersProfile : ObservableDtoViewModelBase<AssisProfileDto> {
	public AssistantUsersProfile(AssisProfileDto dto, AssistantUser user) : base(dto) {
		AsyncCommandMap["Unshare"] = async () => {
			if (!await MessageBox.Show("Unshare Profile", $"Are you sure you want to unshare {Dto.ProfileName}?")) return;
			Toaster.Info("Unsharing profile...");

			_ = await UserAssistantRepo.DeleteAssistantProfile(Dto.id, Dto.ProfileId);
			_ = user.Profilez.Remove(this);
			Toaster.Success($"{Dto.ProfileName} was unshared successfully");
		};
		async Task onSendCookies(SystemBrowserType bt) {
			var email =
				user.Dto.id == Auther.AuthSession?.UserId
				 ? DB.I.Userz.Users?.First(u => u.LicenseKey != null).Email
				 : user.Dto.EmailAddress;
			var cookies = await user.All.First(x => x.Dto.id == Dto.ProfileId).GetCookiesAsync(bt);
			await DB.I.Cooky.SendCookies(
				 Dto.ProfileId,
				 email ?? throw new InvalidOperationException("User email not found"),
				 cookies ?? throw new InvalidOperationException("Failed to get cookies from profile"));
		}
		AsyncCommandMap["SyncCookiesChrome"] = async () => await onSendCookies(SystemBrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = async () => await onSendCookies(SystemBrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = async () => await onSendCookies(SystemBrowserType.Firefox);
	}
}
public partial class AssistantUsersFolder : DtoViewModelBase<AssisShareFolderDto> {
	public AssistantUsersFolder(AssisShareFolderDto dto, AssistantUser user) : base(dto) {
		AsyncCommandMap["Unshare"] = async () => {
			if (!await MessageBox.Show("Unshare Folder", $"Are you sure you want to unshare {Dto.FolderName}? This will not affect other folders.")) return;
			Toaster.Info("Unsharing folder...");
			_ = await ShareFoldersRepo.Instance.Delete(Dto.id);
			_ = user.Folderz.Remove(this);
			Toaster.Success($"{Dto.FolderName} was unshared successfully");
		};
	}
}

public partial class AssistantUser : DtoViewModelBase<AssistDto> {
	[ObservableProperty] bool active;

	public ReadOnlyObservableCollection<ObsProfile> All { get; }
	public ObservableCollection<AssistantUsersProfile> Profilez { get; } = [];
	public ObservableCollection<AssistantUsersFolder> Folderz { get; } = [];

	public AssistantUser(AssistDto dto) : base(dto) {
		_ = UserProfilesRepo.Connect().Transform(i => new ObsProfile(i))
			 .Bind(out var all)
			 .Subscribe();
		All = all;
		Active = DB.I.Userz.Users?.Any(u => u.Email == Dto.EmailAddress) == true;

		AsyncCommandMap["Edit"] = async () => {
			if (await new InviteUserOrAddProfilesViewModel().ShowDialog(Profilez, Folderz) is not { } result) return;
			// Profilez
			await Profilez.Empty(async x =>
				 !result.SelectedProfiles.Any(p => p.Dto.id == x.Dto.ProfileId)
				 && (await UserAssistantRepo.DeleteAssistantProfile(Dto.id, x.Dto.ProfileId)).success
			);
			await EX.Try(async () => {
				await UserAssistantRepo.AddProfiles(Dto.id, result.SelectedProfiles
					 .Where(p => !Profilez.Any(profile => profile.Dto.ProfileId == p.Dto.id))
					 .Select(p => p.Dto.id)
				);
				await InitProfiles();
				Toaster.Success($"profile(s) shared successfully");
			}, ex => {
				Toaster.Error($"Failed to share profile(s). {ex.Message}.");
			});

			// Folderz
			await Folderz.Empty(async folder =>
				 result.SelectedFolders.Any(f => f.Dto.id == folder.Dto.id)
				 && (await ShareFoldersRepo.Instance.Delete(folder.Dto.id)).success
			);
			await EX.Try(async () => {
				await ShareFoldersRepo.Share(Dto.id, result.SelectedFolders
					 .Where(f => !Folderz.Any(folder => folder.Dto.FolderId == f.Dto.id))
					 .Select(f => f.Dto!.id)
				);
				await InitFolders();
				Toaster.Success($"folder(s) shared successfully");
			}, ex => {
				Toaster.Error($"Failed to share folder(s). {ex.Message}.");
			});
		};
		AsyncCommandMap["Copy"] = async () => {
			await CopyPasta.Copy($"{Dto.EmailAddress} {Dto.UserName} {Dto.Password}");
		};
		AsyncCommandMap["Add"] = async () => {
			Active = await DB.I.Userz.Activate(Dto.EmailAddress!);
			Toaster.Success($"User {Dto!.UserName} active status toggled successfully");
		};
		AsyncCommandMap["Remove"] = async () => {
			Active = await DB.I.Userz.Delete(Dto.EmailAddress!) == false;
			Toaster.Success($"User {Dto!.UserName} deactive status toggled successfully");
		};
		AsyncCommandMap["Delete"] = async () => {
			if (!await MessageBox.Show("Delete User", $"Are you sure you want to delete {Dto!.UserName}", icon: "Delete")) return;
			_ = await UserAssistantRepo.Instance.Delete(Dto!.id);
		};
	}
	//
	public override async Task Init(object? param) {
		await base.Init(param);
		if (!Loaded) {
			await InitProfiles();
			await InitFolders();
		}
	}

	private async Task InitProfiles() {
		var profiles = await UserAssistantRepo.GetAllAssistantProfilesById(Dto!.id);
		Profilez.Clear();
		Profilez.AddRange(profiles
			.Where(p => !Profilez.Any(existing => existing.Dto.ProfileId == p.ProfileId))
			.Select(p => new AssistantUsersProfile(p, this)));
	}

	private async Task InitFolders() {
		var folders = await ShareFoldersRepo.GetAll(Dto.id);
		Folderz.Clear();
		Folderz.AddRange(folders.Select(f => new AssistantUsersFolder(f, this)));
	}
}
public partial class TenantMembersViewModel : ViewModelObjectBase {
	[ObservableProperty] int totalCount;

	public ReadOnlyObservableCollection<AssistantUser> Assistantz { get; }
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public TenantMembersViewModel() : base("Members") {
		_ = UserAssistantRepo.Instance.ObservableCache.Connect().Transform(p => {
			var vim = new AssistantUser(p);
			_ = vim.Init(p);
			return vim;
		})
			.Bind(out var assistants)
			.Subscribe((i) => {
				if (assistants != null) {
					TotalCount = assistants.Count;
				}
			});
		Assistantz = assistants;

		_ = UserProfilesRepo.Connect().Transform(i => new ObsProfile(i,
				selectedChanged: p => {
					var obs = Profiles?.FirstOrDefault(x => x.Dto.id == p.Dto.id);
					if (obs == null) return;

					if (p.IsSelected && !SelectedProfiles.Contains(p)) SelectedProfiles.Add(obs);
					else if (SelectedProfiles.Contains(p)) _ = SelectedProfiles.Remove(obs);
				}) { IsActionOptionsVisible = false, IsShowCheckboxColumn = true })
			.SortAndBind(out var profiles, Profiler.AscendingComparer)
			.Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect().Transform(i => new ObsFolder(i))
			.SortAndBind(out var folders, Folderer.AscendingComparer)
			.Subscribe();
		Folders = folders;

		AsyncCommandMap["Add"] = async () => {
			if (Assistantz.Count >= Auther.AuthSession?.LicenseLimits.MaxAssistantsCount)
				throw new InvalidOperationException("You have reached the maximum number of assistants allowed by your license.");
			if (await new InviteUserOrAddProfilesViewModel(true).ShowDialog() is not { } result)
				throw new InvalidOperationException("Failed to invite user or add profiles.");

			var profileIds = result.SelectedProfiles.Select(p => p.Dto.id).ToList();
			var folderIds = result.SelectedFolders.Select(f => f.Dto.id).ToList();
			_ = await UserAssistantRepo.Instance.Create(new AssistDto {
				UserName = result.AssistantName,
				EmailAddress = result.AssistantEmail,
				ProfileIds = profileIds,
				ProfilePermissionIds = [],
				FolderIds = folderIds,
				FolderPermissionIds = []
			});
		};
	}

	public override async Task Init(object? param) {
		await base.Init(param);
		if (!Loaded) await UserAssistantRepo.Instance.Load();
	}
}

