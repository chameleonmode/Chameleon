
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;

using Chameleon.client.Features.Projects.Folders;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class MoveUserProfilesPopupViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private ObsFolder? selectedFolder;
	[ObservableProperty]
	private bool listIsVisible = true;

	public ObservableCollection<ObsFolder> Folders { get; } = [];
	public ObservableCollection<ObsProfile> Profiles { get; } = [];

	public bool HasSelected => SelectedFolder != null;

	partial void OnSelectedFolderChanged(ObsFolder? value) => OnPropertyChanged(nameof(HasSelected));

	[RelayCommand]
	private void SelectFolder(ObsFolder selectedFolder) {
		SelectedFolder = selectedFolder;
	}
}
