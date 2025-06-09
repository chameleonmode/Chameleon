using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;

namespace Chameleon.client.Features.Projects.Folders;

public class FolderChangedEventArgs(UPFolderDto? newCurrentFolderDto) : EventArgs {
	public UPFolderDto? NewCurrentFolderDto { get; } = newCurrentFolderDto;
}

public partial class FoldersViewModel : Projector {
	public static SortExpressionComparer<ObsFolder> AscendingComparer => SortExpressionComparer<ObsFolder>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsFolder> DescendingComparer => SortExpressionComparer<ObsFolder>.Descending(p => p.Dto!.title!);

	public event Action<ObservableDtoViewModelBase<UPFolderDto>>? OnSelectedChanged;

	[ObservableProperty] ObsFolder selectedFolder;

	public ObsFolder AllProfiles { get; }

	public FoldersViewModel() {
		_ = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(folder: i, onSelectedChanged: (folder) => OnSelectedChanged?.Invoke(folder));
		})
		.SortAndBind(out var folders, AscendingComparer)
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
	}

	public async Task OnNavigatingTo(UPFolderDto? p = null) {
		if (p != null) {
			Folders.ForEach(f => f.IsSelected = f.Dto.id == p.id);
			if (Folders.Any(vm => vm.Dto.id == p.id)) {
				await ProfilesViewModel.Instance.OpenAsync(p);
				Instance.SetSelectedFolder(p);
			}
		} else {
				if (AllProfiles != null && !AllProfiles.Navigated) {
					AllProfiles.Navigated = true;
					SetSelectedFolder(AllProfiles.Dto);
				}
			}
	}

	public async void SetSelectedById(int id) {
		_ = await LoadedTCS.Task;
		await OnNavigatingTo(Folders.FirstOrDefault(m => m.Dto?.id == id)?.Dto);
	}

	internal void SetSelectedFolder(UPFolderDto? value) {
		SelectedFolder = value == null
		? AllProfiles
		: Folders.FirstOrDefault(vm => vm.Dto!.id == value.id) ?? AllProfiles;
	}

	public static FoldersViewModel Instance { get; } = new FoldersViewModel();
}

