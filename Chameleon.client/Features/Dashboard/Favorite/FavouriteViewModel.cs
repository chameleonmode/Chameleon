using System.Collections.ObjectModel;
using Chameleon.client.Features.Projects;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using DynamicData;

namespace Chameleon.client.Features.Dashboard.Favorite;
public partial class FavouriteViewModel : Dashboarder {
	public override ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public override ReadOnlyObservableCollection<ObsFolder> Folders { get; }
	public FavouriteViewModel() : base("Favourites") {
		_ = ProfilesViewModel.Instance.Shared.Filter(p => p.Dto.isFavourite)     // only favourites
		.Transform(i => new ObsProfile(i.Dto) { IsShowCheckboxColumn = false })
		.SortAndBind(out var profiles, Profiler.CompareObservable)
		.Subscribe(_ => OnPropertyChanged(nameof(HasProfiles)));
		Profiles = profiles;
		// _ = UserProfilesRepo.Connect(i => i.isFavourite)
		// 	.Transform(i => new ObsProfile(i){ IsShowCheckboxColumn = false})
		// 	.SortAndBind(out var list, profilesCompareObservable)
		// 	.Subscribe((i) => {
		// 		OnPropertyChanged(nameof(HasNoItems));
		// 	});
		// Profiles = list;

		_ = UserProfilesFolderRepo.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i) { IsActionOptionsVisible = true })
			.SortAndBind(out var flist, Folderer.CompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasFolders));
			});
		Folders = flist;
	}

	public static FavouriteViewModel Instance { get; } = new FavouriteViewModel();
}
