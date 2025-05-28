using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Services;

public class FolderChangedEventArgs(UPFolderDto? newCurrentFolderDto) : EventArgs {
	public UPFolderDto? NewCurrentFolderDto { get; } = newCurrentFolderDto;
}

public class FolderManagementService {
	public static SortExpressionComparer<ObsFolder> AscendingComparer => SortExpressionComparer<ObsFolder>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsFolder> DescendingComparer => SortExpressionComparer<ObsFolder>.Descending(p => p.Dto!.title!);

	private readonly ReadOnlyObservableCollection<ObsFolder> allFolders;
	public ReadOnlyObservableCollection<ObsFolder> AllFolders => allFolders;

	public UPFolderDto? CurrentFolderDto { get; private set; }
	public event EventHandler<FolderChangedEventArgs>? CurrentFolderChanged;

	FolderManagementService() {
		_ = UserProfilesFolderRepo.Connect()
				.Transform(dto => new ObsFolder(dto, false, null, null))
				.SortAndBind(out allFolders, AscendingComparer)
				.DisposeMany()
				.Subscribe();
	}
	public static FolderManagementService Instance { get; } = new FolderManagementService();

	public async Task SetCurrentFolderAsync(UPFolderDto? folderDto) {

		if ((CurrentFolderDto?.id == folderDto?.id
			&& CurrentFolderDto != null
			&& folderDto != null)
			|| (CurrentFolderDto == null && folderDto == null)) {
			return;
		}
		CurrentFolderDto = folderDto;

		OnCurrentFolderChanged(new FolderChangedEventArgs(CurrentFolderDto));

		await Task.CompletedTask;//Keep async incase we need to load profiles when folder opens
	}

	protected virtual void OnCurrentFolderChanged(FolderChangedEventArgs e) {
		CurrentFolderChanged?.Invoke(this, e);
	}
}
