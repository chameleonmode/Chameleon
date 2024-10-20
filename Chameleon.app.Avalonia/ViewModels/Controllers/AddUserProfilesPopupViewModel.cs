using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.CommunityToolkit.MvvM;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;

public partial class PupUserProfileViewModel(ObsProfile userProfile) : ViewModelObjectBase(userProfile.Title) {
	public Action? OnSelectedChange { get; set; }

	[ObservableProperty]
	private bool isSelected;

	public ObsProfile UserProfile { get; } = userProfile;

	public char Code => string.IsNullOrWhiteSpace(Title) ? '0' : Title[0];

	[RelayCommand]
	private void Unselect()
	{
		IsSelected = false;
	}

	partial void OnIsSelectedChanged(bool value)
	{
		OnSelectedChange?.Invoke();
	}
}

public partial class AddUserProfilesPupViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private IEnumerable<PupUserProfileViewModel>? selectedViewModels;
	[ObservableProperty]
	private ObsFolder? folder;
	public ObservableCollection<PupUserProfileViewModel> Profiles { get; } = [];

	public bool HasSelected => SelectedViewModels?.Count() > 0;

	partial void OnSelectedViewModelsChanged(IEnumerable<PupUserProfileViewModel>? value) => OnPropertyChanged(nameof(HasSelected));
}

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
	private void SelectFolder(ObsFolder selectedFolder)
	{
		SelectedFolder = selectedFolder;
	}
}
