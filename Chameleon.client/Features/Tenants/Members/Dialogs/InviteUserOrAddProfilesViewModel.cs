using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.client.MvvM;
using Chameleon.lib.Util;
using Chameleon.lib.Helpers;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Api.Dto;

namespace Chameleon.client.Features.Tenants.Members.Dialogs;

public partial class InviteUserOrAddProfilesViewModel : ViewModelObjectBase {
	[ObservableProperty] string? assistantName;
	[ObservableProperty] string? assistantEmail;
	[ObservableProperty] bool showUserInfo;
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
				if (p.IsSelected) {
					if (!SelectedProfiles.Contains(p)) {
						SelectedProfiles.Add(p);
					}
				} else {
					SelectedProfiles.Remove(p);
				}
			}) { IsActionOptionsVisible = false })
		.Bind(out var profiles).Subscribe();
		Profiles = profiles;

		_ = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(i,
			selectedChanged: x => {
				if (x.IsSelected) {
					if (!SelectedFolders.Contains(x)) {
						SelectedFolders.Add(x);
					}
				} else {
					SelectedFolders.Remove(x);
				}
				Profiles.Where(p => p.Dto.folderId == i.id).ForEach(item => {
					item.IsSelected = x.IsSelected;
				});
			}) { IsActionOptionsVisible = false };})
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
		return result == TaskDialogResult.OK ? this : null;
	}
	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog(
		IEnumerable<AssisProfileDto> profilez,
		IEnumerable<AssisShareFolderDto> folderz
	) {
		SelectedProfiles.Clear();
		Profiles.ForEach(i => {
			i.IsSelected = profilez.Any(x => x.ProfileId == i.Dto.id);
		});
		SelectedFolders.Clear();
		Folders.ForEach(i => {
			i.IsSelected = folderz.Any(x => x.FolderId == i.Dto.id);
		});

		return await ShowDialog();
	}
}