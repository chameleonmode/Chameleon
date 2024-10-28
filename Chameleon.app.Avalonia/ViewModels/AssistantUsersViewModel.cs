using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using DynamicData;
using Chameleon.lib.Common.Util;
using System.Reactive.Subjects;
using Chameleon.app.Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api;

namespace Chameleon.app.Avalonia.ViewModels;

public partial class AssistantUsersViewModel
			 : ViewModelObjectBase {
	[ObservableProperty]
	private int totalCount;
	private readonly ReadOnlyObservableCollection<ObsAssistantUser> assistants;
	public ReadOnlyObservableCollection<ObsAssistantUser> Assistantz => assistants;

	public AssistantUsersViewModel(
			): base("User Management")
	{
		_ = UserAssistantRepo.Instance.ObservableCache
			.Connect()
			.Transform(p => {
				var vim = new ObsAssistantUser(p);
				_ = vim.InitAsync(p);
				return vim;
			})
			.Bind(out assistants)
			.Subscribe((i) => {
				if (assistants != null) {
					TotalCount = assistants.Count;
				}
			});

		AsyncCommandMap["CreateNewUserAssistant"] = CreateNewUserAssistant;
	}
	public override async Task InitAsync(object? param)
	{
		await base.InitAsync(param);
		if(!Loaded) {
			await UserAssistantRepo.Instance.Load();
		}
	}

	private async Task CreateNewUserAssistant()
	{
		if (Assistantz.Count >= Auther.AuthSession?.LicenseLimits.MaxAssistantsCount) {
			if (await Mbox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
				ProUtil.GoToUrlDefault(Consts.GlobalSettings.PricingUrl);
		} else {
			var invite = new InviteUserOrAddProfilesViewModel() {
				ShowInviteinfo = true,
			};
			if (await Mbox.ShowTaskDialog<InviteUserOrAddProfilesViewModel, InviteUserOrAddProfilesUserControl>(
							initialize: () => invite,
							header: "Invite User",
							subHeader: "Invite new user and customise their access",
							symbas: Enums.Symbas.AddFriend,
							btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.OK) {
				try {
					var profileIds = invite.SelectedProfiles.Select(p => p.Dto!.id).ToList();
					_ = await UserAssistantRepo.Instance.Create(new AssistDto {
						UserName = invite.AssistantName,
						EmailAddress = invite.AssistantEmail,
						ProfileIds = profileIds,
						ProfilePermissionIds = [],
						FolderIds = [],
						FolderPermissionIds = []
					});
				} catch {
					Toaster.ShowErr($"Failed to invite the user. Please try again.");
				}
			}
		}
	}
	//private void SendLicenceKey(string emailAddress, string password)
	//{
	//	var url = $"mailto:{emailAddress}?subject=Chameleon invitation&body=You’ve been invited to Chameleon. Your credentials:%0DEmail: {emailAddress}%0DKey: {password}%0D";
	//	ProUtil.GoToUrlDefault(url);
	//}
}
