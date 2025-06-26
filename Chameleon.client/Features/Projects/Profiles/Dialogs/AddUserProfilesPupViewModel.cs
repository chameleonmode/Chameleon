using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.lib.Api.Repos;
using Chameleon.client.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Chameleon.lib.Helpers;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class AddUserProfilesPupViewModel : ViewModelObjectBase {
	[ObservableProperty] ObsFolder? folder;

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ObservableCollection<ObsProfile> SelectedProfiles { get; } = [];

	public AddUserProfilesPupViewModel() {
		_ = UserProfilesRepo
					.Connect()
					.Transform(i => new ObsProfile(i,
						selectedChanged: p => {
							var obs = Profiles?.FirstOrDefault(x => x.Dto.id == p.Dto.id);
							if (obs == null) return;

							if (p.IsSelected && !SelectedProfiles.Contains(p)) SelectedProfiles.Add(obs);
							else if (SelectedProfiles.Contains(p)) _ = SelectedProfiles.Remove(obs);

						}) { IsActionOptionsVisible = false }
					)
					.SortAndBind(out var profiles, ProfilesViewModel.AscendingComparer)
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

public static class AddProfilesPopup {
	public static async Task<AddUserProfilesPupViewModel?> Show(UPFolderViewModel folder) {
		var addViewModel = new AddUserProfilesPupViewModel { Title = "Add Profiles" };
		return await MessageBox.ShowTaskDialog<AddUserProfilesPopupUserControl, AddUserProfilesPupViewModel>(new(
				Initialize: () => addViewModel,
				Header: addViewModel.Title,
				SubHeader: $"Select profiles you want to add to {folder.Title} folder:",
				Symbas: Symbas.Folder,
				Btns: MBoxButtons.OkCancel)) == TaskDialogResult.OK && addViewModel.SelectedProfiles.Any()
			? addViewModel : null;
	}
}