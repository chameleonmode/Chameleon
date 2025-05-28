using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api;
using Chameleon.lib.Util;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Tenants.Members.ViewModels;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.client.Features.Tenants.Members.Dialogs;
using Chameleon.app.Avalonia.Services;

namespace Chameleon.client.Features.Tenants.Members;
public partial class TenantMembersViewModel : ViewModelObjectBase {
	private readonly UserAssistantRepo userAssistantRepo = UserAssistantRepo.Instance;

	[ObservableProperty]
	private int totalCount;

	// 
	public ReadOnlyObservableCollection<AssistantUser> Assistantz { get; }

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public TenantMembersViewModel() : base("Members") {
		_ = userAssistantRepo.ObservableCache
			.Connect()
			.Transform(p => {
				var vim = new AssistantUser(p);
				_ = vim.InitAsync(p);
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

		_ = UserProfilesRepo.Connect().Transform(i => new ObsProfile(
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
			.SortAndBind(out var profiles, ProfileManagementService.AscendingComparer)
			.Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect().Transform(i => new ObsFolder(i, false, null, null))
			.SortAndBind(out var folders, FolderManagementService.AscendingComparer)
			.Subscribe();
		Folders = folders;

		AsyncCommandMap["CreateNewUserAssistant"] = CreateNewUserAssistant;
	}

	public override async Task InitAsync(object? param) {
		await base.InitAsync(param);
		if (!Loaded) {
			await userAssistantRepo.Load();
		}
	}

	private async Task CreateNewUserAssistant() {
		if (Assistantz.Count >= Auther.AuthSession?.LicenseLimits.MaxAssistantsCount) {
			if (await MessageBox.Show("USERS LIMIT REACHED", "You have reached the maximum number of users."))
				ProcessUtil.OpenBrowser(Consts.PricingUrl);
		} else {
			if (await new InviteUserOrAddProfilesViewModel(true).ShowDialog() is { } result) {
				try {
					ArgumentException.ThrowIfNullOrEmpty(result.AssistantName);
					ArgumentException.ThrowIfNullOrEmpty(result.AssistantEmail);
					var profileIds = result.SelectedProfiles.Select(p => p.Dto!.id).ToList();
					var folderIds = result.SelectedFolders.Select(f => f.Dto!.id).ToList();
					_ = await userAssistantRepo.Create(new AssistDto {
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

