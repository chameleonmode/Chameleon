using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;

namespace Chameleon.client.Features.Projects.Folders;
public partial class FoldersViewModel : Folderer {
	public ObsFolder? SelectedFolder{ get; private set; }
	
	public ObsFolder AllProfiles => Folders[0];

	public FoldersViewModel() {
		AsyncCommandMap["Create"] = async () => {
			var pcount = UserProfilesFolderRepo.Instance.ObservableCache.Items.Count;
			var pname = $"New Folder - {pcount}";
			while (UserProfilesRepo.Instance.ObservableCache.Items.Any(i => i.title == pname))
				pname = $"New Folder - {++pcount}";

			var folder = await UserProfilesFolderRepo.CreateFolder(pname);
			SetSelected(folder.id);
		};
	}

	public async Task OnNavigatingTo(ObsFolder? folder = null) {
		SelectedFolder ??= AllProfiles;
		var navigate = Folders.FirstOrDefault(f => f.Dto.id == folder?.Dto.id) ?? SelectedFolder;
		if (navigate != SelectedFolder || ProfilesViewModel.Instance.Folder?.Id != navigate.Dto.id) {
			SelectedFolder.IsSelected = false;
			await ProfilesViewModel.Instance.OpenAsync(navigate.Dto);
			SelectedFolder = navigate;
		}
		SelectedFolder.IsSelected = true;
		OnPropertyChanged(nameof(SelectedFolder));
	}

	public async void SetSelected(int id) {
		_ = await LoadedTCS.Task;
		await OnNavigatingTo(Folders.FirstOrDefault(m => m.Dto?.id == id));
	}

	public static FoldersViewModel Instance { get; } = new FoldersViewModel();
}

