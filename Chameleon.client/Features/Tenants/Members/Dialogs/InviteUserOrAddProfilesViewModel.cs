using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;

namespace Chameleon.client.Features.Tenants.Members.Dialogs;

public partial class InviteUserOrAddProfilesViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private string? assistantName;
	[ObservableProperty]
	private string? assistantEmail;
	[ObservableProperty]
	private bool showUserInfo;

	public ProfilesViewModel ProfileService => ProfilesViewModel.Instance;
	public FoldersViewModel FolderService => FoldersViewModel.Instance;

	//
	public ReadOnlyObservableCollection<ObsProfile> Profiles => ProfileService.Profiles;
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	//
	public ReadOnlyObservableCollection<ObsFolder> Folders => FolderService.Folders;
	public ObservableCollection<ObsFolder> SelectedFolders { get; } = [];

	public InviteUserOrAddProfilesViewModel(bool userInfo = false) : base("Select Profiles & Folders") {
		ShowUserInfo = userInfo;

		Profiles.ForEach(profile => {
			profile.IsActionOptionsVisible = false;
			if (profile.IsSelected) SelectedProfiles.Add(profile);
			profile.OnSelectedChanged += p => {
				var obs = Profiles.FirstOrDefault(x => x.Dto.id == p.Dto.id) ?? new ObsProfile(p.Dto);

				if (p.IsSelected && !SelectedProfiles.Contains(p)) SelectedProfiles.Add(obs);
				else if (SelectedProfiles.Contains(obs)) _ = SelectedProfiles.Remove(obs);
			};
		});

		Folders.ForEach(folder => {
			if (folder.IsSelected) SelectedFolders.Add(folder);
			folder.OnSelectedChanged += f => {
				var obs = Folders.FirstOrDefault(x => x.Dto.id == f.Dto.id) ?? new ObsFolder(f.Dto);
				if (f.IsSelected && !SelectedFolders.Contains(f)) SelectedFolders.Add(obs);
				else if (SelectedFolders.Contains(f)) _ = SelectedFolders.Remove(obs);

				Profiles.Where(p => p.Dto!.folderId == f.Dto!.id).ForEach(item => {
					item.IsSelected = f.IsSelected;
				});
			};
		});
	}

	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog() {
		var result = await MessageBox.ShowTaskDialog<InviteUserOrAddProfilesUserControl, InviteUserOrAddProfilesViewModel>(new(
			Initialize: () => this,
			Header: "Select Profiles & Folders",
			SubHeader: "Add profiles and folders to your selection.",
			Symbas: Symbas.AddFriend,
			Btns: MBoxButtons.OkCancel)
		);
		return result == TaskDialogResult.OK ? this : null;
	}
	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog(
		IEnumerable<AssisProfileDto> profilez,
		IEnumerable<AssisShareFolderDto> folderz
	) {
		profilez.ForEach(p => {
			if(Profiles.FirstOrDefault(x => x.Dto.id == p.ProfileId) is { } profile) {
				profile.IsSelected = true;
			}
		});
		folderz.ForEach(f => {
			if(Folders.FirstOrDefault(x => x.Dto.id == f.FolderId) is { } folder) {
				folder.IsSelected = true;
			}
		});
		return await ShowDialog();
	}
}