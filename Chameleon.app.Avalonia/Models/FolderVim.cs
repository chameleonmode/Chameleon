using Chameleon.Infrastructure.UserProfileFolders;
using Chameleon.Interfaces.App.UserProfileFolders.Events;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.CommunityToolkit.MvvM;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.app.Avalonia.Models;
public partial class FolderVim : Vim<UPFolderDto> {
	[ObservableProperty]
	private bool isFavorite;
	[ObservableProperty]
	private int profilesCount;

	public FolderVim(UPFolderDto folder)
			: base(folder.title)
	{
		Dto = folder;
		isFavorite = Dto.isFavorite;
		profilesCount = Dto.profilesCount;

		_ = EventAggregator
				.GetEvent<ChangeProfilesInFavoriteFolderEvent>()
				.Subscribe(args => OnChangeProfilesInFavoriteFolder(args.FolderId));

		CommandMap["SetFavoriteFolder"] = SetFavoriteFolder;
		CommandMap["ViewGroup"] = ViewGroup;
	}

	private async void OnChangeProfilesInFavoriteFolder(int folderId)
	{
		if (Dto.id != folderId) {
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

		Dto.isFavorite = IsFavorite;
		_ = UserProfilesFolderRepo.Instance.Put(Dto);
	}
}
