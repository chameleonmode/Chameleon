using System.Collections.ObjectModel;

using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Playwright.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DynamicData;

namespace Chameleon.app.Avalonia.Models.Observable;
/// <summary>
/// 
/// </summary>
/// <param name="dto"></param>
/// <param name="onProfileUnshare"></param>
/// <param name="onSendCookies"></param>
public partial class ObsAssisProfile
	: Vim<AssisProfileDto> {

	private readonly Action<ObsAssisProfile> onProfileUnshare;
	private readonly Func<ObsAssisProfile, Enums.SystemBrowserType, Task> onSendCookies;

	public ObsAssisProfile(
		AssisProfileDto dto,
		Action<ObsAssisProfile> onProfileUnshare,
		Func<ObsAssisProfile, Enums.SystemBrowserType, Task> onSendCookies) 
		: base(dto)
	{
		this.onProfileUnshare = onProfileUnshare;
		this.onSendCookies = onSendCookies;

		AsyncCommandMap["Unshare"] = Unshare;
		AsyncCommandMap["SyncCookiesChrome"] = () => SendCookies(Enums.SystemBrowserType.Chrome);
		AsyncCommandMap["SyncCookiesBrave"] = () => SendCookies(Enums.SystemBrowserType.Brave);
		AsyncCommandMap["SyncCookiesFirefox"] = () => SendCookies(Enums.SystemBrowserType.Firefox);
	}

	private async Task Unshare()
	{
		if (Dto == null) return;

		try {
			if (await Mbox.Show("Unshare Profile", $"Are you sure you want to unshare {Dto.ProfileName}? This will not affect other profiles.")) {
				Toaster.ShowInf("Unsharing profile...");
				onProfileUnshare(this);
				Toaster.ShowSuccess($"{Dto.ProfileName} was unshared successfully");
			}
		} catch {
			Toaster.ShowErr($"Failed to unshare {Dto.ProfileName}. Please try again.");
		}
	}

	public async Task SendCookies(Enums.SystemBrowserType bt)
	{
		try {
			Toaster.ShowInf("Sending cookies...");
			await onSendCookies(this, bt);
			Toaster.ShowSuccess("Cookies sent successfully");
		} catch {
			Toaster.ShowErr($"Failed to send cookies.");
		}
	}
}

public partial class ObsAssisFolder(
	AssisShareFolderDto dto,
	Action<ObsAssisFolder> onFolderUnshare)
	: Vim<AssisShareFolderDto>(dto) {

	[RelayCommand]
	public async Task Unshare()
	{
		if (Dto == null) return;

		try {
			if (await Mbox.Show("Unshare Folder", $"Are you sure you want to unshare {Dto.FolderName}? This will not affect other folders.")) {
				Toaster.ShowInf("Unsharing folder...");
				onFolderUnshare(this);
				Toaster.ShowSuccess($"{Dto.FolderName} was unshared successfully");
			}
		} catch {
			Toaster.ShowErr($"Failed to unshare {Dto.FolderName}. Please try again.");
		}
	}
}

/// <summary>
/// 
/// </summary>
/// <param name="dto"></param>
public partial class ObsAssistantUser(AssistDto dto) : Vim<AssistDto>(dto) {
	// 
	private readonly PlaywrightCookiesRepo _playwrightCookiesRepo = PlaywrightCookiesRepo.Instance;

	[ObservableProperty]
	private bool canCreateProfiles;

	//
	public ObservableCollection<ObsAssisProfile> Profilez { get; } = [];
	public ObservableCollection<ObsAssisFolder> Folderz { get; } = [];

	//
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		if (!Loaded) {
			await InitProfiles();
			await InitFolders();
		}
	}

	private async Task InitProfiles()
	{
		var profiles = await UserAssistantRepo.GetAllAssistantProfilesById(Dto!.id);
		Profilez.Clear();
		Profilez.AddRange(profiles.Select(p => new ObsAssisProfile(p,
			onProfileUnshare: async op => {
				var (userId, dtoId) = EnsureDtoIds(Dto!.id, op.Dto!.ProfileId);
				_ = await UserAssistantRepo.DeleteAssistantProfile(userId, dtoId);
				_ = Profilez.Remove(op);
			},
			onSendCookies: async (op, bt) => {
				var (userId, dtoId) = EnsureDtoIds(
					Dto!.id == Auther.AuthSession?.UserId && Auther.AuthSession?.CreatorUserId != null
						? Auther.AuthSession.CreatorUserId
						: Dto?.id,
						op.Dto?.ProfileId);
				await _playwrightCookiesRepo.PutChromiumCookies(userId.ToString(), dtoId.ToString(), bt);
			}
		)));
	}

	private async Task InitFolders()
	{
		var folders = await ShareFoldersRepo.GetAll(Dto!.id);
		Folderz.Clear();
		Folderz.AddRange(folders.Select(f => new ObsAssisFolder(f,
			onFolderUnshare: async of => {
				var (userId, dtoId) = EnsureDtoIds(Dto!.id, of.Dto!.FolderId);
				_ = await ShareFoldersRepo.Instance.Delete(dtoId);
				_ = Folderz.Remove(of);
			}
		)));
	}

	private static (long userId, int dtoId) EnsureDtoIds(long? user, int? profile)
  {
    ArgumentNullException.ThrowIfNull(user);
    ArgumentNullException.ThrowIfNull(profile);
    return (user.Value, profile.Value);
  }

	partial void OnCanCreateProfilesChanged(bool value)
	{
		Dto!.CanCreateProfiles = value;
		SetCanCreateProfiles();
	}

	[RelayCommand]
	private async Task DeleteAssistant()
	{
		if (await Mbox.Show("Delete User", $"Are you sure you want to delete {Dto!.UserName}", fontIconInfo: "Delete")) {
			try {
				_ = await UserAssistantRepo.Instance.Delete(Dto!.id);
			} catch {
				Toaster.ShowErr($"Failed to delete {Dto!.UserName}. Please try again.");
			}
		}
	}

	[RelayCommand]
	private async Task AddMoreProfiles()
	{
		try {
			var invite = new InviteUserOrAddProfilesViewModel() {
				ShowUserInfo = false,
			};
			if (await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
				initialize: () => invite,
				header: "Add Profiles",
				subHeader: "Add access to specific Profiles for this user",
				symbas: Enums.Symbas.AddFriend,
				btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
				var profileIds = invite.SelectedProfiles.Select(p => p.Dto!.id).ToList();
				var result = await UserAssistantRepo.AddProfiles(Dto!.id, profileIds, []);
				await InitProfiles();
				Toaster.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
			}
		} catch (Exception ex) {
			Toaster.ShowErr($"Failed to share profile(s). {ex.Message}.");
		}
	}

	[RelayCommand]
	private async Task SendLicenceKey()
	{
		await CopyPasta.Copy($"{Dto!.EmailAddress} {Dto!.UserName} {Dto!.Password}");
	}

	[RelayCommand]
	private void SetCanCreateProfiles()
	{
		try {
			_ = UserAssistantRepo.SetCanCreateProfiles(Dto!.id, Dto!.CanCreateProfiles);

			Toaster.ShowSuccess($"Permission to create profiles was successfully {(Dto!.CanCreateProfiles ? "given" : "taken")}");
		} catch {
			Toaster.ShowErr($"Create profiles permission update failed. Please try again.");
		}
	}
}
