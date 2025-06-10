using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.client.MvvM;
using Chameleon.lib.Util;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.client.Features.Tenants.Members.Dialogs;

public partial class InviteUserOrAddProfilesViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private string? assistantName;
	[ObservableProperty]
	private string? assistantEmail;
	[ObservableProperty]
	private bool showUserInfo;
	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; } 
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; } 
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public InviteUserOrAddProfilesViewModel(bool userInfo = false) : base("Select Profiles & Folders") {
		ShowUserInfo = userInfo;
		_ = UserProfilesRepo.Connect()
		.Transform(i => new ObsProfile(i,
			selectedChanged: p => {
				var obs = ProfilesViewModel.Instance.Profiles.FirstOrDefault(x => x.Dto.id == i.id) ?? new ObsProfile(i);
				if (p.IsSelected && !SelectedProfiles.Contains(p)) SelectedProfiles.Add(obs);
				else if (!p.IsSelected && SelectedProfiles.Contains(p)) _ = SelectedProfiles.Remove(obs);
		}){ IsActionOptionsVisible = false })
		.Bind(out var profiles).Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(i,
			 onSelectedChanged: f => {
				var obs = FoldersViewModel.Instance.Folders.FirstOrDefault(x => x.Dto.id == i.id) ?? new ObsFolder(i);
				if (f.IsSelected && !SelectedFolders.Contains(f)) SelectedFolders.Add(obs);
				else if (!f.IsSelected && SelectedFolders.Contains(f)) _ = SelectedFolders.Remove(obs);

				Profiles.Where(p => p.Dto.folderId == i.id).ForEach(item => {
					item.IsSelected = f.IsSelected;
				});
			}){ IsActionOptionsVisible = false };
		})
		.Bind(out var folders).Subscribe();
		Folders = folders;
	}

	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog() {
		var result = await MessageBox.ShowTaskDialog<InviteUserOrAddProfilesUserControl, InviteUserOrAddProfilesViewModel>(new(
			Initialize: () => this,
			Header: "Select Profiles & Folders",
			SubHeader: "Add profiles and folders to your selection.",
			Symbas: Symbas.AddFriend,
			Btns: MBoxButtons.OkCancel)
		);
		ProfilesViewModel.Instance.Profiles.ForEach(p => p.IsActionOptionsVisible = p.IsShowCheckboxColumn = true);
		return result == TaskDialogResult.OK ? this : null;
	}
	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog(
		IEnumerable<AssisProfileDto> profilez,
		IEnumerable<AssisShareFolderDto> folderz
	) {
		Profiles.ForEach(i => {
			i.IsSelected = profilez.Any(x => x.ProfileId == i.Dto.id);
		});
		Folders.ForEach(i => {
			i.IsSelected = folderz.Any(x => x.FolderId == i.Dto.id);
		});
		return await ShowDialog();
	}
}