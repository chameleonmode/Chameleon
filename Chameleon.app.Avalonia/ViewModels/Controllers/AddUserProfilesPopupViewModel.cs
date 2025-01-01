using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.CommunityToolkit.MvvM;
using System.Collections.ObjectModel;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Api.Repos;
using DynamicData;
using Chameleon.lib.Common.Extensions;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class AddUserProfilesPupViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private ObsFolder? folder;

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	public AddUserProfilesPupViewModel()
	{
		_ = UserProfilesRepo
					.Connect()
					.Transform(i => new ObsProfile(
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
					.SortAndBind(out var profiles, Compares.ObsProfileCompares.AscendingComparer)
					.Subscribe(async p => {
						var pre = SelectedProfiles.ToList();
						SelectedProfiles.Clear();
						await Task.Delay(64);
						foreach (var item in pre) {
							var cp = Profiles?.First(pr => pr.Dto!.id == item.Dto!.id);
							if (cp != null) {
								cp.IsSelected = true;
								SelectedProfiles.Add(cp);
							}
						}
					});
		Profiles = profiles;
	}
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
