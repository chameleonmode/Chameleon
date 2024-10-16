using CommunityToolkit.Mvvm.ComponentModel;

using Chameleon.lib.CommunityToolkit.MvvM;
using System.Reactive.Linq;
using DynamicData;
using Chameleon.lib.Api.Repos;
using System.Collections.ObjectModel;
using Chameleon.lib.Common.Constants;
using Chameleon.app.Avalonia.Com.DynamicData;
using System.Reactive.Subjects;
using Chameleon.app.Avalonia.Models.Observable;

namespace Chameleon.app.Avalonia.ViewModels;
public partial class DashboardViewModel : ViewModelObjectBase {
	private readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.UserProfileVimCompares.AscendingComparer);
	private readonly BehaviorSubject<IComparer<ObsFolder>> foldersCompareObservable = new(Compares.FolderVimCompares.AscendingComparer);

	[ObservableProperty]
	private bool isSyncChangesBtnVisible = true;
	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;
	[ObservableProperty]
	private Enums.ChangeComparereOption folderSortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; }

	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;

	public DashboardViewModel() 
		: base("Dashboard")
	{
		//OnSortSelectedChanged(Enums.ChangeComparereOption.Ascending);
		_ = UserProfilesRepo
			.Connect(i => i.isFavourite)
			.Transform(i => new ObsProfile(i, false))
			.SortAndBind(out var list, profilesCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoItems)); 
			});
		Profiles = list;

		//OnFolderSortSelectedChanged(Enums.ChangeComparereOption.Ascending);
		_ = UserProfilesFolderRepo
			.Connect(i => i.isFavorite)
			.Transform(i => new ObsFolder(i))
			.SortAndBind(out var flist, foldersCompareObservable)
			.Subscribe((i) => {
				OnPropertyChanged(nameof(HasNoFolderItems));
			});
		Folders = flist;

		AsyncCommandMap["SyncChanges"] = SyncChanges;
	}

	partial void OnSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		profilesCompareObservable.OnNext(value switch { 
			Enums.ChangeComparereOption.Descending => Compares.UserProfileVimCompares.DescendingComparer,
			_ => Compares.UserProfileVimCompares.AscendingComparer
		});
	}

  partial void OnFolderSortSelectedChanged(Enums.ChangeComparereOption value)
	{
		foldersCompareObservable.OnNext(value switch {
			Enums.ChangeComparereOption.Descending => Compares.FolderVimCompares.DescendingComparer,
			_ => Compares.FolderVimCompares.AscendingComparer
		});
	}

	private async Task SyncChanges()
	{
		await AppStartup.Instance.LoadSink();
	}
}

