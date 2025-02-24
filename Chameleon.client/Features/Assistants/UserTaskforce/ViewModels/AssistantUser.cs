using System.Collections.ObjectModel;
using System.Diagnostics;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Helpers;
using Chameleon.lib.Playwright;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;

namespace Chameleon.app.Features.Assistants.UserTaskforce.ViewModels;
public partial class AssistantUsersProfile : ObservableViewModelDto<AssisProfileDto> {
	private readonly Action<AssistantUsersProfile> onProfileUnshare;
	private readonly Func<AssistantUsersProfile, Enums.SystemBrowserType, Task> onSendCookies;

	public AssistantUsersProfile(
		AssisProfileDto dto,
		Action<AssistantUsersProfile> onProfileUnshare,
		Func<AssistantUsersProfile, Enums.SystemBrowserType, Task> onSendCookies)
		: base(dto) {
		this.onProfileUnshare = onProfileUnshare;
		this.onSendCookies = onSendCookies;

		AsyncCommandMap["Unshare"] = Unshare;
		AsyncCommandMap["SyncCookiesChrome"] = () => SendCookies(Enums.SystemBrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = () => SendCookies(Enums.SystemBrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = () => SendCookies(Enums.SystemBrowserType.Firefox);
	}

	private async Task Unshare() {
		if (Dto == null) return;

		try {
			if (await Mbox.Show("Unshare Profile", $"Are you sure you want to unshare {Dto.ProfileName}? This will not affect other profiles.")) {
				Toaster.Info("Unsharing profile...");
				onProfileUnshare(this);
				Toaster.Success($"{Dto.ProfileName} was unshared successfully");
			}
		} catch {
			Toaster.Error($"Failed to unshare {Dto.ProfileName}. Please try again.");
		}
	}

	public async Task SendCookies(Enums.SystemBrowserType bt) {
		try {
			Toaster.Info("Sending cookies...");
			await onSendCookies(this, bt);
		} catch {
			Toaster.Error($"Failed to send cookies.");
		}
	}
}
public partial class AssistantUsersFolder(AssisShareFolderDto dto, Action<AssistantUsersFolder> onFolderUnshare)
	: ViewModelObjectDto<AssisShareFolderDto>(dto) {

	[RelayCommand]
	public async Task Unshare() {
		if (Dto == null) return;

		try {
			if (await Mbox.Show("Unshare Folder", $"Are you sure you want to unshare {Dto.FolderName}? This will not affect other folders.")) {
				Toaster.Info("Unsharing folder...");
				onFolderUnshare(this);
				Toaster.Success($"{Dto.FolderName} was unshared successfully");
			}
		} catch {
			Toaster.Error($"Failed to unshare {Dto.FolderName}. Please try again.");
		}
	}
}

public partial class AssistantUser(AssistDto dto)  : ViewModelObjectDto<AssistDto>(dto) {
	[ObservableProperty]
	private bool canCreateProfiles;
	//
	public ObservableCollection<AssistantUsersProfile> Profilez { get; } = [];
	public ObservableCollection<AssistantUsersFolder> Folderz { get; } = [];

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
		Profilez.AddRange(profiles.Select(p => new AssistantUsersProfile(p,
			onProfileUnshare: async op => {
				var (userId, dtoId) = EnsureDtos(Dto!.id, op.Dto!.ProfileId);
				_ = await UserAssistantRepo.DeleteAssistantProfile(userId, dtoId);
				_ = Profilez.Remove(op);
			},
			onSendCookies: async (op, bt) => {
				var profile = GetRunningProfile(op.Dto!.ProfileId) ?? await GetNewProfileAsync(op.Dto!.ProfileId);
				Process? getBrowserProfileProcess() => GetBrowserProfileProcess(profile);
				void openBrowserProfile(Enums.SystemBrowserType browserType) {
					if (browserType == Enums.SystemBrowserType.Firefox) {
						profile.OpenFirefoxCommand.Execute(null);
						return;
					}
					if (browserType == Enums.SystemBrowserType.Brave) {
						profile.OpenBraveCommand.Execute(null);
						return;
					} 
					profile.OpenChromeCommand.Execute(null);
				}

				var cookies = await PlaywrightUtil.GetCookies(op.Dto!.ProfileId.ToString()!, bt, openBrowserProfile, getBrowserProfileProcess);
				if (cookies.Count > 0) {
					var platformaticDB = PlatformaticDB.Instance;
					var email = Dto!.id == Auther.AuthSession?.UserId
					? platformaticDB.DBusers?.SingleOrDefault(u => u.licenseKey != null)?.email
					: Dto!.EmailAddress;
					var data = await platformaticDB.SendCookies(email!, op.Dto!.ProfileId.ToString(), cookies);
					if (data != null) {
						Toaster.Success($"Cookies sent successfully");
					} else {
						Toaster.Error($"Failed to send cookies");
					}
				} else {
					Toaster.Info("No cookies to send in the local profile cache");
				}

				async Task<ObsProfile> GetNewProfileAsync(int profileId) {
					var profile = await UserProfilesRepo.GetProfileById(profileId);
					return new ObsProfile(profile);
				}

				ObsProfile? GetRunningProfile(int profileId) {
					return MyProfilesViewModel.Instance.Profiles.FirstOrDefault(x => x.Dto!.id == profileId);
				}

				Process? GetBrowserProfileProcess(ObsProfile profile) {
					var browser = profile.SBI[bt];
					return browser?.Brocess;
				}
			}
		)));
	}

	private async Task InitFolders() {
		var folders = await ShareFoldersRepo.GetAll(Dto!.id);
		Folderz.Clear();
		Folderz.AddRange(folders.Select(f => new AssistantUsersFolder(f,
			onFolderUnshare: async of => {
				var (userId, dtoId) = EnsureDtos(Dto!.id, of.Dto!.id);
				_ = await ShareFoldersRepo.Instance.Delete(dtoId);
				_ = Folderz.Remove(of);
			}
		)));
	}

	private static (long userId, int dtoId) EnsureDtos(long? user, int? profile) {
		ArgumentNullException.ThrowIfNull(user);
		ArgumentNullException.ThrowIfNull(profile);
		return (user.Value, profile.Value);
	}

	partial void OnCanCreateProfilesChanged(bool value) {
		Dto!.CanCreateProfiles = value;
		SetCanCreateProfiles();
	}

	[RelayCommand]
	private async Task DeleteAssistant() {
		if (await Mbox.Show("Delete User", $"Are you sure you want to delete {Dto!.UserName}", fontIconInfo: "Delete")) {
			try {
				_ = await UserAssistantRepo.Instance.Delete(Dto!.id);
			} catch {
				Toaster.Error($"Failed to delete {Dto!.UserName}. Please try again.");
			}
		}
	}

	[RelayCommand]
	private async Task AddMoreProfiles() {
		try {
			var invite = new InviteUserOrAddProfilesViewModel() {
				ShowUserInfo = false,
			};
			if (
        await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
				initialize: () => invite,
				header: "Add Profiles",
				subHeader: "Add access to specific Profiles for this user",
				symbas: Enums.Symbas.AddFriend,
				btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK
      ) {
				var profileIds = invite.SelectedProfiles.Select(p => p.Dto!.id).ToList();
				if (profileIds.Count != 0) {
					var result = await UserAssistantRepo.AddProfiles(Dto!.id, profileIds, []);
					await InitProfiles();
					Toaster.Success($"{profileIds.Count} profile(s) shared successfully");
				}
				//
				var folderIds = invite.SelectedFolders.Select(f => f.Dto!.id).ToList();
				if (folderIds.Count != 0) {
					var folderResult = await ShareFoldersRepo.Share(Dto!.id, folderIds, []);
					await InitFolders();
					Toaster.Success($"{folderIds.Count} folder(s) shared successfully");
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
