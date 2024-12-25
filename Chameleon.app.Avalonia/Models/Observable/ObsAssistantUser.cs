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
public partial class ObsAssisProfile(AssisProfileDto dto, Action<ObsAssisProfile> onProfileUnshare, Func<ObsAssisProfile, Task> onSendCookies) : Vim<AssisProfileDto>(dto) {
	[RelayCommand]
	public void Unshare()
	{
		onProfileUnshare(this);
	}

	[RelayCommand]
	public async Task SendCookies()
	{
		await onSendCookies(this);
	}
}
public partial class ObsAssistantUser(AssistDto dto) : Vim<AssistDto>(dto) {
	private readonly PlaywrightCookiesRepo _playwrightCookiesRepo = PlaywrightCookiesRepo.Instance;

	[ObservableProperty]
	private bool canCreateProfiles;
	public ObservableCollection<ObsAssisProfile> Profilez { get; } = [];
	public ObservableCollection<ObsFolder> Folderz { get; } = [];

	public AssistDto AssistDto => Dto!;

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		if (!Loaded) {
			await InitProfiles();
		}
	}

	private async Task InitProfiles()
	{
		var profiles = await UserAssistantRepo.GetAllAssistantProfilesById(Dto!.id);
		AddProfiles(profiles);
	}

	private void AddProfiles(
		AssisProfileDto[] profiles
	) => Profilez.AddRange(profiles.Select(p =>
		new ObsAssisProfile(p
			, onProfileUnshare: async op => {
				if (await Mbox.Show("Unshare Profile", $"Are you sure you want to unshare {p.ProfileName}? This will not affect other profiles.")) {
					try {
						_ = await UserAssistantRepo.DeleteAssistantProfile(Dto!.id, op.Dto!.ProfileId);

						_ = Profilez.Remove(op);

						Toaster.ShowSuccess($"{op.Dto!.ProfileName} was unshared successfully");
					} catch {
						Toaster.ShowErr($"Failed to unshare profile. Please try again.");
					}
				}
			}
			, onSendCookies: async op => {
				try {
					var userId =
						AssistDto.id == Auther.AuthSession!.UserId && Auther.AuthSession!.CreatorUserId != null
							? Auther.AuthSession.CreatorUserId.ToString()
							: AssistDto.id.ToString();

					if (userId != null) {
						await _playwrightCookiesRepo.PutChromiumCookies(
							userId,
							op.Dto!.ProfileId!.ToString(),
							Enums.SystemBrowserType.Chrome
						);
					} else {
						Toaster.ShowErr("Failed to send cookies. User ID is null.");
					}
				} catch (Exception e) {
					Toaster.ShowErr($"Failed to send cookies. {e.Message}");
				}
			}
		)
	));

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
		var invite = new InviteUserOrAddProfilesViewModel();
		if (await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
						initialize: () => invite,
						header: "Add Profiles",
						subHeader: "Add access to specific Profiles for this user",
						symbas: Chameleon.lib.Common.Constants.Enums.Symbas.AddFriend,
						btns: Chameleon.lib.Common.Constants.Enums.MBoxButtons.OkCancel) == Chameleon.lib.Common.Constants.Enums.TaskDialogResult.OK) {
			try {
				var profileIds = invite.SelectedProfiles.Select(p => p.Dto!.id).ToList();
				var result = await UserAssistantRepo.AddProfiles(Dto!.id, profileIds, []);
				await InitProfiles();

				Toaster.ShowSuccess($"{profileIds.Count} profile(s) shared successfully");
			} catch (Exception ex) {
				Toaster.ShowErr($"Failed to share profile(s). {ex.Message}.");
			}
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
