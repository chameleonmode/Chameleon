using Chameleon.app.Avalonia.Models.Observable;
using System.Collections.ObjectModel;

using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Api.Repos;
using DynamicData;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles;
using Chameleon.app.Avalonia.Features.ProfilesAndFolders.Folders;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Util;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;

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

		MyProfilesViewModel.Instance.PaginatorViewModel.UpdatePageCount(UserProfilesRepo.Instance.ObservableCache.Count);
		MyProfilesViewModel.Instance.Profiles.ForEach(p => {
			p.IsActionOptionsVisible = false;
			if (p.IsSelected) SelectedProfiles.Add(p);
		});
		MyProfilesViewModel.Instance.OnSelectedChanged += OnProfileSelectedChanged;
		Profiles = MyProfilesViewModel.Instance.Profiles;

		FoldersViewModel.Instance.Folders.ForEach(f => {
			if (f.IsSelected) SelectedFolders.Add(f);
		});
		FoldersViewModel.Instance.OnSelectedChanged += OnFolderSelectedChanged;
		Folders = FoldersViewModel.Instance.Folders;
	}

	void OnProfileSelectedChanged(ObsProfile p) {
		if (p.IsSelected) {
			if (!SelectedProfiles.Contains(p))
				SelectedProfiles.Add(p);
		} else {
			_ = SelectedProfiles.Remove(p);
		}
	}

	void OnFolderSelectedChanged(ObsFolder f) {
		if (f.IsSelected) {
			if (!SelectedFolders.Contains(f)) {
				SelectedFolders.Add(f);
			}
		} else {
			_ = SelectedFolders.Remove(f);
		}

		var profiles = MyProfilesViewModel.Instance.Profiles.Where(p => p.Dto!.folderId == f.Dto!.id);
		if (profiles != null) {
			foreach (var item in profiles) {
				item.IsSelected = f.IsSelected;
			}
		}
	}

	public async Task<InviteUserOrAddProfilesViewModel?> ShowDialog() {
		var result = await Mbox.ShowTaskDialog<Controls.InviteUserOrAddProfilesUserControl, InviteUserOrAddProfilesViewModel>(new(
			Initialize: () => this,
			Header: "Select Profiles & Folders",
			SubHeader: "Add profiles and folders to your selection.",
			Symbas: Enums.Symbas.AddFriend,
			Btns: Enums.MBoxButtons.OkCancel)
		);
		MyProfilesViewModel.Instance.Profiles.ForEach(p => p.IsActionOptionsVisible = true);
		MyProfilesViewModel.Instance.PaginatorViewModel.UpdatePageCount(Consts.PageinationPageItems);
		return result == Enums.TaskDialogResult.OK ? this : null;
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