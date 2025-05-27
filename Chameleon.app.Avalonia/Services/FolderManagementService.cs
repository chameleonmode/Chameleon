using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using DynamicData;
using System.Collections.ObjectModel;

namespace Chameleon.app.Avalonia.Services;
public class FolderChangedEventArgs(UPFolderDto? newCurrentFolderDto) : EventArgs {
	public UPFolderDto? NewCurrentFolderDto { get; } = newCurrentFolderDto;
}

public interface IFolderManagementService {
	ReadOnlyObservableCollection<ObsFolder> AllFolders { get; }
	SourceCache<UPFolderDto, int> FolderDtoCache { get; }

	Task EnsureFoldersLoadedAsync();

	UPFolderDto? CurrentFolderDto { get; }

	Task SetCurrentFolderAsync(UPFolderDto? folderDto);

	event EventHandler<FolderChangedEventArgs>? CurrentFolderChanged;
}

public class FolderManagementService : IFolderManagementService {
	private readonly UserProfilesFolderRepo folderRepo;
	private bool areFoldersLoaded = false;
	private readonly object _loadLock = new();

	private readonly ReadOnlyObservableCollection<ObsFolder> allFolders;
	public ReadOnlyObservableCollection<ObsFolder> AllFolders => allFolders;
	public SourceCache<UPFolderDto, int> FolderDtoCache => folderRepo.SourceCache;

	private UPFolderDto? currentFolderDto;
	public UPFolderDto? CurrentFolderDto => currentFolderDto;
	public event EventHandler<FolderChangedEventArgs>? CurrentFolderChanged;

	public FolderManagementService(UserProfilesFolderRepo folderRepo) {
		this.folderRepo = folderRepo;
		_ = FolderDtoCache.Connect()
				.Transform(dto => new ObsFolder(dto, false, null, null))
				.SortAndBind(out allFolders, Compares.ObsFolderCompares.AscendingComparer)
				.DisposeMany()
				.Subscribe();
	}

	public async Task EnsureFoldersLoadedAsync() {
		var load = false;
		lock (_loadLock) {
			areFoldersLoaded = allFolders.Any();
			load = !areFoldersLoaded;
		}
		if (load) {
			var dtos = await folderRepo.GetAll<UPFolderDto>();
			FolderDtoCache.Edit(update => {
				update.Clear();
				update.AddOrUpdate(dtos);
			});
			lock (_loadLock) areFoldersLoaded = true;
		}
	}

	public async Task SetCurrentFolderAsync(UPFolderDto? folderDto) {

		if ((currentFolderDto?.id == folderDto?.id 
			&& currentFolderDto != null 
			&& folderDto != null) 
			|| (currentFolderDto == null && folderDto == null)) {
			return;
		}
		currentFolderDto = folderDto;

		OnCurrentFolderChanged(new FolderChangedEventArgs(currentFolderDto));

		await Task.CompletedTask;//Keep async incase we need to load profiles when folder opens
	}

	protected virtual void OnCurrentFolderChanged(FolderChangedEventArgs e) {
		CurrentFolderChanged?.Invoke(this, e);
	}

	public static FolderManagementService Instance { get; } = new FolderManagementService(UserProfilesFolderRepo.Instance);
}
