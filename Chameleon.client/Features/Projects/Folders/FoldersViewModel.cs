using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.app.Avalonia.Services;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using UserProfilesViewModel = Chameleon.client.Features.ProfilesAndFolders.Profiles.MyProfiles.MyProfilesViewModel;

namespace Chameleon.client.Features.ProfilesAndFolders.Folders;

public partial class FoldersViewModel : ViewModelObjectBase {

	[ObservableProperty] ObsFolder selectedFolder;

	public ObsFolder AllProfiles { get; }
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public event Action<ObsFolder>? OnSelectedChanged;

	public FoldersViewModel() {
		_ = UserProfilesFolderRepo.Connect()
		.Transform(i => new ObsFolder(
			folder: i,
			hasActionOptions: false,
			onSelectedChanged: f => OnSelectedChanged?.Invoke(f),
			nameAlreadyExist: folderName => Folders?.Any(x => x.Dto.title == folderName) ?? false
		))
		.SortAndBind(out var folders, FolderManagementService.AscendingComparer)
		.Subscribe();
		Folders = folders;
		SelectedFolder = AllProfiles = folders[0];

		AsyncCommandMap["Create"] = async () => {
			var pcount = UserProfilesFolderRepo.Instance.ObservableCache.Items.Count;
			var pname = $"New Folder - {pcount}";
			while (UserProfilesRepo.Instance.ObservableCache.Items.Any(i => i.title == pname))
				pname = $"New Folder - {++pcount}";

			var folder = await UserProfilesFolderRepo.CreateFolder(pname);

			_ = OnNavigatingTo(folder);
		};

		FolderManagementService.Instance.CurrentFolderChanged += FolderManagementService_CurrentFolderChanged;
	}

	private void FolderManagementService_CurrentFolderChanged(object? sender, FolderChangedEventArgs e) {
		_ = OnNavigatingTo(e.NewCurrentFolderDto);
	}

	public async Task OnNavigatingTo(UPFolderDto? p = null) {
		if (p != null) {
			foreach (var item in Folders)
				item.IsSelected = item.Dto!.id == p.id;

			var pvm = Folders.FirstOrDefault(vm => vm.Dto!.id == p.id);
			if (pvm != null) {
				await UserProfilesViewModel.Instance.OpenAsync(p);
			}
		} else {
			if (AllProfiles != null && !AllProfiles.Navigated) {
				AllProfiles.Navigated = true;
				await AllProfiles.AsyncCommandMap["Open"]();
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

	public static FoldersViewModel Instance { get; } = new FoldersViewModel();
}

