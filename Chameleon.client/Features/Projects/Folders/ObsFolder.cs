using Chameleon.client.Services;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.client.MvvM;
using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Projects.Folders;

public partial class ObsFolder : ObservableDtoViewModelBase<UPFolderDto> {

	[ObservableProperty] bool isFavorite;
	[ObservableProperty] int profilesCount;
	[ObservableProperty] bool isRenamed;
	[ObservableProperty] bool isActionOptionsVisible;

	public bool IsContextMenuVisible => Dto!.id != 0;
	public bool ShowFavoriteIcon => IsContextMenuItemEnabled && Dto?.id != 0;
	public bool IsSharedFolder => Dto?.creatorUserId != null && Dto?.creatorUserId != Auther.AuthSession?.UserId;
	public bool IsContextMenuItemEnabled => Auther.AuthSession?.CreatorUserId == null || Auther.AuthSession?.CreatorUserId == Dto?.creatorUserId;
	public bool IsFolderNotEmpty => UserProfilesRepo.Instance.ObservableCache.Items.Any(p => (p.folderId == null && Dto!.id == 0) || p.folderId == Dto!.id);

	public ObsFolder(UPFolderDto folder,  Action<ObservableDtoViewModelBase<UPFolderDto>>? onSelectedChanged = default) 
	: base(folder, onSelectedChanged) {
		isFavorite = Dto.isFavorite;
		profilesCount = Dto.profilesCount;

		CommandMap["StartRename"] = () => {
			Title = Dto?.title;
			IsRenamed = true;
		};
		CommandMap["SetFavoriteFolder"] = () => {
			IsFavorite = !IsFavorite;

			Dto!.isFavorite = IsFavorite;
			_ = UserProfilesFolderRepo.Instance.Put(Dto);
		};
		CommandMap["ViewGroup"] = () => Navigator.Instance.NavigateTo("ProjectsView", this);
		CommandMap["ChangeProxies"] = () => Navigator.Instance.NavigateTo("FunctionalSettingsView", this);

		AsyncCommandMap["Open"] = async () => await FoldersViewModel.Instance.OnNavigatingTo(Dto);
		AsyncCommandMap["SetFavorite"] = async () => {
			IsFavorite = !IsFavorite;
			Dto!.isFavorite = IsFavorite;

			_ = await UserProfilesFolderRepo.Instance.Put(Dto);

			OnPropertyChanged(nameof(Dto));
		};
		AsyncCommandMap["Delete"] = async () => {
			if (await MessageBox.Show("Delete Folder",
					$"Are you sure you want to delete {Dto!.title} folder? This will not affect individual profiles within the folder.",
					MBoxButtons.OkCancel,
					"DeleteLines")) {

				var userProfiles = UserProfilesRepo.Instance.ObservableCache.Items.Where(p => p.folderId == Dto!.id);
				var deletes = new List<Task>();
				foreach (var item in userProfiles) {
					item.folderId = null;
					deletes.Add(UserProfilesRepo.Instance.Put(item));
				}
				await Task.WhenAll(deletes);
				var res = await UserProfilesFolderRepo.Instance.Delete(Dto!.id);
				if (!res.success) throw new InvalidOperationException($"Failed to delete folder {Dto.title}:");
				IsSelected = false;
				FoldersViewModel.Instance.SetSelectedFolder(null);
			}
		};
		AsyncCommandMap["SaveRename"] = async () => {
			ArgumentException.ThrowIfNullOrEmpty(Title, nameof(Title));

			if (FoldersViewModel.Instance.Folders?.Any(x => x.Dto.title == this.Title) == true)
				throw new InvalidOperationException($"Folder named {this.Title} already exists");

			var res = await UserProfilesFolderRepo.Instance.Put(new UPFolderDto {
				id = Dto.id,
				title = Title,
				isFavorite = Dto.isFavorite,
				creatorUserId = Dto.creatorUserId
			});

			Title = res != null ? (Dto.title = res.title) : Dto.title;
			IsRenamed = false;

			// if (wasSelected) await AsyncCommandMap["Open"]();
		};

		UserProfilesRepo.Instance.OnProfileChanged += (profile) => {
			if (profile.folderId == Dto!.id) {
				ProfilesCount = UserProfilesRepo.Instance.ObservableCache.Items.Count(p => p.folderId == Dto!.id);
			}
		};
	}

	// Properties Changed Events
	public override void OnAnyIsSelectedChanged(bool value) {
		base.OnAnyIsSelectedChanged(value);
		IsRenamed = false;
	}
}
