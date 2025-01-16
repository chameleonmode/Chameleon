using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using DynamicData;
using Chameleon.lib.Common.Util;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api;
using Chameleon.app.Avalonia.DynamicData;

namespace Chameleon.app.Avalonia.ViewModels.General;
public partial class AssistantTaskforceViewModel : ViewModelObjectBase {
	private readonly UserAssistantRepo userAssistantRepo = UserAssistantRepo.Instance;

	[ObservableProperty]
	private int totalCount;

	// 
	public ReadOnlyObservableCollection<ObsAssistantUser> Assistantz { get; }

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public ObservableCollection<ObsProfile> SelectedFolders { get; } = [];

	public AssistantTaskforceViewModel() : base("Assistant Outforce")
	{
		_ = userAssistantRepo.ObservableCache
			.Connect()
			.Transform(p => {
				var vim = new ObsAssistantUser(p);
				_ = vim.InitAsync(p);
				return vim;
			})
			.Bind(out var assistants)
			.Subscribe((i) => {
				if (assistants != null) {
					TotalCount = assistants.Count;
				}
			});
		Assistantz = assistants;

		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(
				userProfile: i,
				hasActionOptions: false,
				onSelectedChanged: p => {
					if (p.IsSelected) {
						if (!SelectedProfiles.Contains(p))
							SelectedProfiles.Add(p);
					} else {
						_ = SelectedProfiles.Remove(p);
					}
				})
			)
			.SortAndBind(out var profiles, Compares.ObsProfileCompares.AscendingComparer)
			.Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo
			.Connect()
			.Transform(i => new ObsFolder(i, false, null))
			.SortAndBind(out var folders, Compares.ObsFolderCompares.AscendingComparer)
			.Subscribe();
		Folders = folders;

		AsyncCommandMap["CreateNewUserAssistant"] = CreateNewUserAssistant;
	}

	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		if (!Loaded) {
			await userAssistantRepo.Load();
		}
	}

	private async Task CreateNewUserAssistant()
	{
		if (Assistantz.Count >= Auther.AuthSession?.LicenseLimits.MaxAssistantsCount) {
			if (await Mbox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
				ProUtil.GoToUrlDefault(Consts.PricingUrl);
		} else {
			var invite = new InviteUserOrAddProfilesViewModel();
			if (await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
							initialize: () => invite,
							header: "Invite User",
							subHeader: "Invite new user and customise their access",
							symbas: Enums.Symbas.AddFriend,
							btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
				try {
					ArgumentNullException.ThrowIfNullOrEmpty(invite.AssistantName);
					ArgumentNullException.ThrowIfNullOrEmpty(invite.AssistantEmail);
					var profileIds = invite.SelectedProfiles.Select(p => p.Dto!.id).ToList();
					_ = await userAssistantRepo.Create(new AssistDto {
						UserName = invite.AssistantName,
						EmailAddress = invite.AssistantEmail,
						ProfileIds = profileIds,
						ProfilePermissionIds = [],
						FolderIds = [],
						FolderPermissionIds = []
					});
				} catch {
					Toaster.Error($"Failed to invite the user. Please try again.");
				}
			}
		}
	}
}
