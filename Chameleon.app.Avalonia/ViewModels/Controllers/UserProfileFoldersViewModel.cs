using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Api.Repos;
using Chameleon.app.Avalonia.Models.Observable;
using System.Collections.ObjectModel;
using DynamicData;
using Chameleon.app.Avalonia.Com.DynamicData;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.ViewModels.Controllers;
public partial class UserProfileFoldersViewModel : ViewModelObjectBase {
	[ObservableProperty]
	private ObsFolder? selectedFolder;

	public ObsFolder? AllProfiles { get; private set; }
	private readonly ReadOnlyObservableCollection<ObsFolder> folders;
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;

	private UserProfileFoldersViewModel()
	{
		//selectedFolder = AllProfiles;
		//_ = AllProfiles.Open();
		_ = UserProfilesFolderRepo
		.Connect()
		.Transform(i => new ObsFolder(i, this))
		.SortAndBind(out folders, Compares.ObsFolderCompares.AscendingComparer)
		.Subscribe();
		SelectedFolder = AllProfiles = folders[0];
		_ = SelectedFolder.Open();

		AsyncCommandMap["Create"] = Create;
	}

	private async Task Create()
	{
		var folder = await UserProfilesFolderRepo.CreateFolder($"New Folder - {Folders.Count}");

		_ = OnNavigatingTo(folder);
	}

	public async Task OnNavigatingTo(UPFolderDto? p = null)
	{
		_ = await LoadedTCS.Task;

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

	public async void SetSelectedById(int id)
	{
		_ = await LoadedTCS.Task;

		await OnNavigatingTo(Folders.FirstOrDefault(m => m.Dto?.id == id)?.Dto);
	}

	public static UserProfileFoldersViewModel Instance { get; } = new UserProfileFoldersViewModel();
}
