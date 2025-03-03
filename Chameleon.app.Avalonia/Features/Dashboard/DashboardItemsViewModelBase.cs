using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.CommunityToolkit.MvvM;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace Chameleon.app.Avalonia.Features.Dashboard;
public partial class DashboardItemsViewModelBase : ViewModelObjectBase {

	protected readonly BehaviorSubject<IComparer<ObsProfile>> profilesCompareObservable = new(Compares.ObsProfileCompares.AscendingComparer);
	protected readonly BehaviorSubject<IComparer<ObsFolder>> foldersCompareObservable = new(Compares.ObsFolderCompares.AscendingComparer);

	[ObservableProperty]
	private Enums.ChangeComparereOption sortSelected = Enums.ChangeComparereOption.Ascending;
	[ObservableProperty]
	private Enums.ChangeComparereOption folderSortSelected = Enums.ChangeComparereOption.Ascending;

	public Enums.ChangeComparereOption[] Sorts { get; } = (Enums.ChangeComparereOption[])Enum.GetValues(typeof(Enums.ChangeComparereOption));

	public ReadOnlyObservableCollection<ObsProfile> Profiles { get; protected set; }
	public ReadOnlyObservableCollection<ObsFolder> Folders { get; protected set; }

	public bool HasNoFolderItems => Folders.Count == 0;
	public bool HasNoItems => Profiles.Count == 0;

	public DashboardItemsViewModelBase(string? title) : base(title) {
		Title = title;
		Profiles = new ReadOnlyObservableCollection<ObsProfile>([]);
		Folders = new ReadOnlyObservableCollection<ObsFolder>([]);
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

}
