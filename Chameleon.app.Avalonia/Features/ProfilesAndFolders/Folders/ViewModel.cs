using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;

using UserProfilesViewModel = Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.MyProfiles.ViewModel;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Folders;
public partial class ViewModel : ObservableObjectBase {

	[ObservableProperty]
	private ObsFolder selectedFolder;

	public ObsFolder AllProfiles { get; }
	private readonly ReadOnlyObservableCollection<ObsFolder> folders;
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;

	public ViewModel() {
		_ = UserProfilesFolderRepo
		.Connect()
		.Transform(i => new ObsFolder(i))
		.SortAndBind(out folders, Compares.ObsFolderCompares.AscendingComparer)
		.Subscribe();
		SelectedFolder = AllProfiles = folders[0];

		AsyncCommandMap["Create"] = Create;
	}

	private async Task Create() {
		var pcount = UserProfilesFolderRepo.Instance.ObservableCache.Items.Count;
		var pname = $"New Folder - {pcount}";
		while (UserProfilesRepo.Instance.ObservableCache.Items.Any(i => i.title == pname))
			pname = $"New Folder - {++pcount}";

		var folder = await UserProfilesFolderRepo.CreateFolder(pname);

		_ = OnNavigatingTo(folder);
	}

	public async Task OnNavigatingTo(UPFolderDto? p = null) {
		if (p != null) {
			foreach (var item in Folders)
				item.IsSelected = item.Dto!.id == p.id;

			var pvm = Folders.FirstOrDefault(vm => vm.Dto!.id == p.id);
			if (pvm != null) {
				UserProfilesViewModel.Instance.Open(p);
			}
		} else {
			if (AllProfiles != null && !AllProfiles.Navigated) {
				AllProfiles.Navigated = true;
				await AllProfiles.Open();
			}
		}
	}

	public async void SetSelectedById(int id) {
		_ = await LoadedTCS.Task;

		await OnNavigatingTo(Folders.FirstOrDefault(m => m.Dto?.id == id)?.Dto);
	}

	internal void SetSelectedFolder(UPFolderDto? value) {
		SelectedFolder = value == null ?
			AllProfiles
			: Folders.FirstOrDefault(vm => vm.Dto!.id == value.id) ?? AllProfiles;
	}

	public static ViewModel Instance { get; } = new ViewModel();
}
