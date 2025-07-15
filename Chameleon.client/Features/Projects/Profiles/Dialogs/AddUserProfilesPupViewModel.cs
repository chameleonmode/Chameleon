using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.lib.Helpers;
using Chameleon.lib.Services;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class AddUserProfilesPupViewModel : OOVM {
	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; init; } = [];

	public AddUserProfilesPupViewModel(UPFolderViewModel folder) {
		_ = UserProfilesRepo.Connect().Transform(i => new ObsProfile(i,
				selectedChanged: p => {
					if (p.IsSelected && !SelectedProfiles.Contains(p)) SelectedProfiles.Add(p);
					else if (!p.IsSelected && SelectedProfiles.Contains(p)) _ = SelectedProfiles.Remove(p);
				}) { IsActionOptionsVisible = false, IsSelected = i.folderId == folder.Id }
			).SortAndBind(out var profiles, Profiler.AscendingComparer)
			.Subscribe();
		Profiles = profiles;
	}
}

public static class AddProfilesPopup {
	public static async Task<AddUserProfilesPupViewModel?> Show(UPFolderViewModel folder) {
		var addViewModel = new AddUserProfilesPupViewModel(folder) { Title = "Add Profiles" };
		return await MessageBox.ShowTaskDialog<AddUserProfilesPopupUserControl, AddUserProfilesPupViewModel>(new(
				Initialize: () => addViewModel,
				Header: addViewModel.Title,
				SubHeader: $"Select profiles you want to add to {folder.Title} folder:",
				Symbas: Symbas.Folder,
				Btns: MBoxButtons.OkCancel)) == TaskDialogResult.OK && addViewModel.SelectedProfiles.Any()
			? addViewModel : null;
	}
}