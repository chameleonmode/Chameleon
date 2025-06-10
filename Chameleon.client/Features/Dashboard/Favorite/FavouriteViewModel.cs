using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.client.Features.Dashboard.Favorite;
public partial class FavouriteViewModel : Base {
	public override ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public override ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public FavouriteViewModel() : base("Favourites") {

		_ = ProfilesViewModel.Instance.Shared
				.Filter(p => p.Dto.isFavourite)     // only favourites
				.Transform(i => {
					i.IsShowCheckboxColumn = false;
					return i;
				})
				.SortAndBind(out var favourites, profilesCompareObservable)              // just sort, no paging
				.Subscribe(_ => OnPropertyChanged(nameof(HasNoItems)));
		Profiles = favourites;
		// _ = UserProfilesRepo.Connect(i => i.isFavourite)
		// 	.Transform(i => new ObsProfile(i){ IsShowCheckboxColumn = false})
		// 	.SortAndBind(out var list, profilesCompareObservable)
		// 	.Subscribe((i) => {
		// 		OnPropertyChanged(nameof(HasNoItems));
		// 	});
		// Profiles = list;

		_ = UserProfilesFolderRepo.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i) { IsActionOptionsVisible = true })
			.SortAndBind(out var flist, foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;
	}

	public static FavouriteViewModel Instance { get; } = new FavouriteViewModel();
}
