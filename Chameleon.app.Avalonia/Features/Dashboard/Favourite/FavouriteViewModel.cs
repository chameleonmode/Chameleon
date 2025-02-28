using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.Features.Dashboard.Favourite;
public partial class FavouriteViewModel : ViewModelObjectBase {

	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.ObsProfileCompares.AscendingComparer);
	private readonly BehaviorSubject<IComparer<ObsFolder>> foldersCompareObservable = new(Compares.ObsFolderCompares.AscendingComparer);

	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;
	[ObservableProperty]
	private Enums.ChangeComparereOption folderSortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;

	public FavouriteViewModel() : base("Favourites") {
		_ = UserProfilesRepo
					.Connect(i => i.isFavourite)
					.Transform(i => new ObsProfile(i, false))
					.SortAndBind(out var list, profilesCompareObservable)
					.Subscribe((i) => {
						OnPropertyChanged(nameof(HasNoItems));
					});
		Profiles = list;

		_ = UserProfilesFolderRepo
			.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i, true, null))
			.SortAndBind(out var flist, foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value) {
		profilesCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsProfileCompares.DescendingComparer,
			_ => Compares.ObsProfileCompares.AscendingComparer
		});
	}

	partial void OnFolderSortSelectedChanged(Enums.ChangeComparereOption value) {
		foldersCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.ObsFolderCompares.DescendingComparer,
			_ => Compares.ObsFolderCompares.AscendingComparer
		});
	}

	public static FavouriteViewModel Instance { get; } = new FavouriteViewModel();
}
