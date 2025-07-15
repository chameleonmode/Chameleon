
using Chameleon.client.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;

using Chameleon.client.Features.Projects.Folders;
using DynamicData;
using Chameleon.lib.Helpers;
using Chameleon.lib.Services;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class MoveUserProfilesPopupViewModel : OOVM {
	[ObservableProperty] ObsFolder selectedFolder;
	[ObservableProperty] bool listIsVisible = true;

	public ObservableCollection<ObsFolder> Folders { get; } = [];
	public ObservableCollection<ObsProfile> Profiles { get; } = [];

	public MoveUserProfilesPopupViewModel() : base("Move User Profiles") {
		// Initialize the folders and profiles collections if needed
		Folders.AddRange(FoldersViewModel.Instance.Folders);
		SelectedFolder = Folders.First();
	}

	[RelayCommand]
	private void SelectFolder(ObsFolder selectedFolder) {
		SelectedFolder = selectedFolder;
	}
}
public static class MoveProfilesPopup {
	public static async Task<MoveUserProfilesPopupViewModel?> Show(IEnumerable<ObsProfile> profils) {
		if (profils == null || !profils.Any()) return null;
		var moveViewModel = new MoveUserProfilesPopupViewModel { Title = "Add To Folder" };
		moveViewModel.Profiles.AddRange(profils);
		return await MessageBox.ShowTaskDialog<MoveUserProfilesPopupUserControl, MoveUserProfilesPopupViewModel>(new(
				Initialize: () => moveViewModel,
				Header: moveViewModel.Title,
				SubHeader: $"Select a folder to move the {profils.Count()} selected profiles:",
				Symbas: Symbas.Folder,
				Btns: MBoxButtons.OkCancel)) == TaskDialogResult.OK && moveViewModel.SelectedFolder?.Dto != null ?
			moveViewModel : null;
	}
}
