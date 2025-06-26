using System.Collections.ObjectModel;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;
using Chameleon.lib.Playwright.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Chameleon.client.Features.Tenants.Members.Dialogs;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Dto;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.WebBrowser.Browsers;

namespace Chameleon.client.Features.Tenants.Members.ViewModels;
public partial class AssistantUsersProfile : ObservableDtoViewModelBase<AssisProfileDto> {
	private readonly Action<AssistantUsersProfile> onProfileUnshare;
	private readonly Func<AssistantUsersProfile, SystemBrowserType, Task> onSendCookies;

	public AssistantUsersProfile(
		AssisProfileDto dto,
		Action<AssistantUsersProfile> onProfileUnshare,
		Func<AssistantUsersProfile, SystemBrowserType, Task> onSendCookies)
		: base(dto) {
		this.onProfileUnshare = onProfileUnshare;
		this.onSendCookies = onSendCookies;

		AsyncCommandMap["Unshare"] = Unshare;
		AsyncCommandMap["SyncCookiesChrome"] = () => SendCookies(SystemBrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = () => SendCookies(SystemBrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = () => SendCookies(SystemBrowserType.Firefox);
	}

	private async Task Unshare() {
		if (Dto == null) return;

		try {
			if (await MessageBox.Show("Unshare Profile", $"Are you sure you want to unshare {Dto.ProfileName}? This will not affect other profiles.")) {
				Toaster.Info("Unsharing profile...");
				onProfileUnshare(this);
				Toaster.Success($"{Dto.ProfileName} was unshared successfully");
			}
		} catch {
			Toaster.Error($"Failed to unshare {Dto.ProfileName}. Please try again.");
		}
	}

	public async Task SendCookies(SystemBrowserType bt) {
		try {
			Toaster.Info("Sending cookies...");
			await onSendCookies(this, bt);
		} catch (Exception ex) {
			Toaster.Error($"Failed to send cookies.", ex.Message);
		}
	}
}
public partial class AssistantUsersFolder(AssisShareFolderDto dto, Action<AssistantUsersFolder> onFolderUnshare)
	: DtoViewModelBase<AssisShareFolderDto>(dto) {

	[RelayCommand]
	public async Task Unshare() {
		if (Dto == null) return;

		try {
			if (await MessageBox.Show("Unshare Folder", $"Are you sure you want to unshare {Dto.FolderName}? This will not affect other folders.")) {
				Toaster.Info("Unsharing folder...");
				onFolderUnshare(this);
				Toaster.Success($"{Dto.FolderName} was unshared successfully");
			}
		} catch {
			Toaster.Error($"Failed to unshare {Dto.FolderName}. Please try again.");
		}
	}
}

public partial class AssistantUser : DtoViewModelBase<AssistDto> {

	private readonly ReadOnlyObservableCollection<ObsProfile> allProfiles;
	[ObservableProperty] bool canCreateProfiles;
	[ObservableProperty] bool isNotActive;
	//
	public ObservableCollection<AssistantUsersProfile> Profilez { get; } = [];
	public ObservableCollection<AssistantUsersFolder> Folderz { get; } = [];

	public AssistantUser(AssistDto dto) : base(dto) {
		_ = UserProfilesRepo
	.Connect()
	.Transform(i => new ObsProfile(i))
	.Bind(out allProfiles)
	.Subscribe();
	}
	//
	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
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
			.Select(p => new AssistantUsersProfile(p,
			onProfileUnshare: async op => {
				_ = await UserAssistantRepo.DeleteAssistantProfile(Dto.id, op.Dto.ProfileId);
				_ = Profilez.Remove(op);
			}, onSendCookies: async (op, bt) => {
				var profile = allProfiles.FirstOrDefault(x => x.Dto!.id == op.Dto!.ProfileId)
				?? throw new InvalidOperationException("Profile not found");

				var cookies = await profile.GetCookiesAsync(bt);

				if (cookies?.Count > 0) {
					await DB.Instance.EnsureUser();
					var email = Dto!.id != Auther.AuthSession?.UserId ? Dto!.EmailAddress
						: DB.Instance.DBusers?.SingleOrDefault(u => u.LicenseKey != null)?.Email;
					var data = await DB.Routes.Cooky.SendCookies(email!, op.Dto!.ProfileId.ToString(), cookies);
					if (data != null) {
						Toaster.Success($"Cookies sent successfully ({cookies.Count} cookies)");
					} else {
						Toaster.Error($"Failed to send cookies to server");
					}
				} else {
					Toaster.Info("No cookies found in the browser profile");
				}
			}
		)));
	}

	private async Task InitFolders() {
		var folders = await ShareFoldersRepo.GetAll(Dto!.id);
		Folderz.Clear();
		Folderz.AddRange(folders.Select(f => new AssistantUsersFolder(f,
			onFolderUnshare: async of => {
				_ = await ShareFoldersRepo.Instance.Delete(of.Dto.id);
				_ = Folderz.Remove(of);
			}
		)));
	}

	partial void OnCanCreateProfilesChanged(bool value) {
		Dto!.CanCreateProfiles = value;
		SetCanCreateProfiles();
	}

	[RelayCommand]
	private async Task DeleteAssistant() {
		if (await MessageBox.Show("Delete User", $"Are you sure you want to delete {Dto!.UserName}", icon: "Delete")) {
			try {
				_ = await UserAssistantRepo.Instance.Delete(Dto!.id);
			} catch {
				Toaster.Error($"Failed to delete {Dto!.UserName}. Please try again.");
			}
		}
	}

	[RelayCommand]
	async Task ToggleActive() {
		try {
			IsNotActive = (await DB.Instance.CreateUser(Dto!.EmailAddress!)) != null;
			Toaster.Success($"User {Dto!.UserName} active status toggled successfully");
		} catch (Exception e) {
			Toaster.Error($"Failed to toggle user {Dto!.UserName} active status, {e.Message[e.Message.LastIndexOf('\n')..]}");
		}
	}

	[RelayCommand]
	async Task ToggleDeActive() {
		try {
			_ = await DB.Instance.DeleteUser(Dto!.EmailAddress!);
			IsNotActive = DB.Instance.DBusers?.Any(u => u.Email == Dto?.EmailAddress) == true;
			Toaster.Success($"User {Dto!.UserName} deactive status toggled successfully");
		} catch (Exception e) {
			Toaster.Error($"Failed to toggle user {Dto!.UserName} deactive status, {e.Message[e.Message.LastIndexOf('\n')..]}");
		}
	}

	[RelayCommand]
	private async Task AddMoreProfiles() {
		try {
			if (
				await new InviteUserOrAddProfilesViewModel().ShowDialog(
					Profilez.Select(p => p.Dto), Folderz.Select(f => f.Dto)
				) is { } result
			) {
				// Profilez
				await Profilez.Empty(
					async x => !result.SelectedProfiles.Any(p => p.Dto.id == x.Dto.ProfileId)
					 && (await UserAssistantRepo.DeleteAssistantProfile(Dto.id, x.Dto.ProfileId)).success
				);

				var profilesToAdd = result.SelectedProfiles
					.Where(p => !Profilez.Any(profile => profile.Dto.ProfileId == p.Dto.id))
					.Select(p => p.Dto!.id)
					.ToList();

				if (profilesToAdd.Count != 0 &&
					(await UserAssistantRepo.AddProfiles(Dto.id, profilesToAdd))?.success == true) {
					await InitProfiles();
					Toaster.Success($"profile(s) shared successfully");
				}

				// Folderz
				await Folderz.Empty(
					async folder => result.SelectedFolders.Any(f => f.Dto.id == folder.Dto.id)
					&& (await ShareFoldersRepo.Instance.Delete(folder.Dto.id)).success
				);

				var foldersToAdd = result.SelectedFolders
					.Where(f => !Folderz.Any(folder => folder.Dto.FolderId == f.Dto.id))
					.Select(f => f.Dto!.id)
					.ToList();

				if (foldersToAdd.Count != 0 &&
					(await ShareFoldersRepo.Share(Dto.id, foldersToAdd)).Length != 0) {
					await InitFolders();
					Toaster.Success($"folder(s) shared successfully");
				}
			}
		} catch (Exception ex) {
			Toaster.Error($"Failed to share profile(s). {ex.Message}.");
		}
	}

	[RelayCommand]
	private async Task SendLicenceKey() {
		await CopyPasta.Copy($"{Dto!.EmailAddress} {Dto!.UserName} {Dto!.Password}");
	}

	[RelayCommand]
	private void SetCanCreateProfiles() {
		try {
			_ = UserAssistantRepo.SetCanCreateProfiles(Dto!.id, Dto!.CanCreateProfiles);

			Toaster.Success($"Permission to create profiles was successfully {(Dto!.CanCreateProfiles ? "given" : "taken")}");
		} catch {
			Toaster.Error($"Create profiles permission update failed. Please try again.");
		}
	}
}
