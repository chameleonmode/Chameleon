using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.client.Features.Dashboard.Favorite;
public partial class FavouriteViewModel : Base {
	public FavouriteViewModel() : base("Favourites") {
		_ = UserProfilesRepo.Connect(i => i.isFavourite)
			.Transform(i => new ObsProfile(i){ IsShowCheckboxColumn = false})
			.SortAndBind(out var list, profilesCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoItems));
			});
		Profiles = list;

		_ = UserProfilesFolderRepo.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i){ IsActionOptionsVisible = true })
			.SortAndBind(out var flist, foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;
	}

	public static FavouriteViewModel Instance { get; } = new FavouriteViewModel();
}
