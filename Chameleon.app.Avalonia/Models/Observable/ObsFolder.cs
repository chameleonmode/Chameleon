using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.Interfaces.App.Synchronization.Events;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.FunctionalSettings;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.app.Avalonia.Models.Observable;
public partial class ObsFolder : Vim<UPFolderDto> {
	private readonly UserProfileFoldersViewModel? foldervm;

	[ObservableProperty]
	private bool isFavorite;
	[ObservableProperty]
	private int profilesCount;
	[ObservableProperty]
	private bool isFavoriteButtonVisible = true;
	[ObservableProperty]
	private bool isRenamed;

	private bool _isSelected;

	public bool IsSelected {
		get => _isSelected;
		set {
			SetProperty(ref _isSelected, value);
			if (value == false) {
				IsRenamed = false;
			} else {
				//foldervm.SelectedFolder = this;
			}
		}
	}

	public bool IsFolderNotEmpty => UserProfilesRepo.Instance.ObservableCache.Items
		.Where(profiles => profiles.folderId == Dto!.id)
		.Any();

	public ObsFolder(UPFolderDto folder, UserProfileFoldersViewModel? foldervm = null)
			: base(folder.title)
	{
		Dto = folder;
		this.foldervm = foldervm;

		isFavorite = Dto.isFavorite;
		profilesCount = Dto.profilesCount;

		_ = EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Subscribe(args => OnChangeProfilesInFavoriteFolder(args.FolderId));

		CommandMap["SetFavoriteFolder"] = SetFavoriteFolder;
		CommandMap["ViewGroup"] = ViewGroup;
		this.foldervm = foldervm;
	}

	private async void OnChangeProfilesInFavoriteFolder(int folderId)
	{
		if (Dto?.id != folderId) {
			return;
		}
		Dto = await UserProfilesFolderRepo.Instance.Get<UPFolderDto>(folderId);
		ProfilesCount = UserProfilesRepo.Instance.ObservableCache.Items.Count(a => a.id == folderId);
		IsFavorite = Dto.isFavorite;
		OnPropertyChanged(nameof(ProfilesCount));
	}

	private void ViewGroup()
	{
		Navigator.NavigateToType(typeof(IProjectsView), Dto);
	}

	private void SetFavoriteFolder()
	{
		IsFavorite = !IsFavorite;

		Dto!.isFavorite = IsFavorite;
		_ = UserProfilesFolderRepo.Instance.Put(Dto);
	}

	[RelayCommand]
	public async Task Open()
	{
		if(foldervm != null)
			await foldervm.OnNavigatingTo(Dto);
			IsSelected = true;
	}

	[RelayCommand]
	private async Task SetFavorite()
	{
		IsFavorite = !IsFavorite;

		Dto!.isFavorite = IsFavorite;

		var res = await UserProfilesFolderRepo.Instance.Put(Dto);
		EventAggregator
				.GetEvent<UpdateFavoriteFolderEvent>()
				.Publish();

		EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Publish(new ChangeProfilesInFavoriteFolderEventArgs(Dto.id));

		OnPropertyChanged(nameof(Dto));
	}

	[RelayCommand]
	private async Task Delete()
	{
		if (await Mbox.Show("Delete Folder",
				$"Are you sure you want to delete {Dto!.title} folder? This will not affect individual profiles within the folder.",
				Enums.MBoxButtons.OkCancel,
				"DeleteLines")) {

			await Task.Run(async () => {
				var userProfiles = UserProfilesRepo.Instance.ObservableCache.Items;
				var deletes = new List<Task>();
				foreach (var item in userProfiles) {
					item.folderId = null;
					deletes.Add(Task.Run(() => UserProfilesRepo.Instance.Put(item)));
				}
				await Task.WhenAll(deletes);
				var res = await UserProfilesFolderRepo.Instance.Delete(Dto!.id);
			});

			EventAggregator
					.GetEvent<AfterCreateOrRemoveFolderEvent>()
					.Publish();

			await foldervm!.AllProfiles.Open();
		}
	}

	[RelayCommand]
	private void StartRename()
	{
		Title = Dto?.title;
		IsRenamed = true;
	}

	[RelayCommand]
	private void ChangeProxies()
	{
		Navigator.NavigateToType(typeof(IFunctionalSettingsView), this);
	}

	[RelayCommand]
	private async Task SaveRename()
	{
		if (string.IsNullOrEmpty(Title)) {
			return;
		}

		var orignalTitle = Dto!.title;
		try {
			Dto.title = Title;
			var res = await UserProfilesFolderRepo.Instance.Put(Dto);

			EventAggregator
					.GetEvent<RenameFolderEvent>()
					.Publish(new RenameFolderEventArgs(Dto.id, Dto.title));
		} catch {
			Dto.title = orignalTitle;
		}

		Title = Dto.title;

		IsRenamed = false;

		EventAggregator
			 .GetEvent<SyncChangesEvent>()
			 .Publish();
	}
}
