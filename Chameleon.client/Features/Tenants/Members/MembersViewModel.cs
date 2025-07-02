using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Api;
using Chameleon.lib.Util;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Tenants.Members.ViewModels;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.client.Features.Tenants.Members.Dialogs;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Tenants.Members;
public partial class TenantMembersViewModel : ViewModelObjectBase {
	[ObservableProperty] int totalCount;

	// 
	public ReadOnlyObservableCollection<AssistantUser> Assistantz { get; }

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public TenantMembersViewModel() : base("Members") {
		_ = UserAssistantRepo.Instance.ObservableCache.Connect().Transform(p => {
				var vim = new AssistantUser(p);
				_ = vim.Init(p);
				vim.IsNotActive = DB.Instance.DBusers?.Any(u => u.Email == p.EmailAddress) ?? false;
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
			.SortAndBind(out var profiles, ProfilesViewModel.AscendingComparer)
			.Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect().Transform(i => new ObsFolder(i))
			.SortAndBind(out var folders, FoldersViewModel.AscendingComparer)
			.Subscribe();
		Folders = folders;

		AsyncCommandMap["CreateNewUserAssistant"] = CreateNewUserAssistant;
	}

	public override async Task Init(object? param) {
		await base.Init(param);
		if (!Loaded) await UserAssistantRepo.Instance.Load();
	}

	private async Task CreateNewUserAssistant() {
		if (Assistantz.Count >= Auther.AuthSession?.LicenseLimits.MaxAssistantsCount) {
			if (await MessageBox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
				ProcessUtil.OpenBrowser(Const.PricingUrl);
		} else {
			if (await new InviteUserOrAddProfilesViewModel(true).ShowDialog() is { } result) {
				try {
					ArgumentException.ThrowIfNullOrEmpty(result.AssistantName);
					ArgumentException.ThrowIfNullOrEmpty(result.AssistantEmail);
					var profileIds = result.SelectedProfiles.Select(p => p.Dto!.id).ToList();
					var folderIds = result.SelectedFolders.Select(f => f.Dto!.id).ToList();
					_ = await UserAssistantRepo.Instance.Create(new AssistDto {
						UserName = result.AssistantName,
						EmailAddress = result.AssistantEmail,
						ProfileIds = profileIds,
						ProfilePermissionIds = [],
						FolderIds = folderIds,
						FolderPermissionIds = []
					});
				} catch {
					Toaster.Error($"Failed to invite the user. Please try again.");
				}
			}
		}
	}
}

