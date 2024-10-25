using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.CommunityToolkit.MvvM;
using System.Collections.ObjectModel;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class AddUserProfilesPupViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private IEnumerable<ObsProfile>? selectedViewModels;
	[ObservableProperty]
	private ObsFolder? folder;

	public ObservableCollection<ObsProfile> Profiles { get; } = [];
	public bool HasSelected => SelectedViewModels?.Count() > 0;

	public AddUserProfilesPupViewModel()
	{
		_ = UserProfilesRepo
			.Connect()
			.Transform(i => new ObsProfile(i, onSelectedChanged: OnSelectedChanged))
			.Filter(i=> i.Dto?.folderId == null)
			.SortAndBind(out var list, Compares.ObsProfileCompares.AscendingComparer)
			.Subscribe();
		foreach (var item in list) {
			Profiles.Add(item);
		}
	}

	private void OnSelectedChanged() => SelectedViewModels = Profiles.Where(i => i.IsSelected);
	partial void OnSelectedViewModelsChanged(IEnumerable<ObsProfile>? value) => OnPropertyChanged(nameof(HasSelected));
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
