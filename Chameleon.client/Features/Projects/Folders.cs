using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.MvvM;
using Chameleon.lib.Api.Repos;
using DynamicData;
using DynamicData.Binding;

namespace Chameleon.client.Features.Projects;

public abstract partial class Folderer : OOVM {
	public static SortExpressionComparer<ObsFolder> AscendingComparer => SortExpressionComparer<ObsFolder>.Ascending(p => p.Dto!.title!);
	public static SortExpressionComparer<ObsFolder> DescendingComparer => SortExpressionComparer<ObsFolder>.Descending(p => p.Dto!.title!);
	public static BehaviorSubject<IComparer<ObsFolder>> CompareObservable { get; } = new(AscendingComparer);

	public IObservable<IChangeSet<ObsFolder, int>> Shared { get; }
	public virtual ReadOnlyObservableCollection<ObsFolder> Folders { get; }
  
	public bool HasNoItems => Folders.Count == 0;
	public int SelectedCount => Folders.Where(i => i.IsSelected)?.Count() ?? 0;
	public bool HasSelectedItems => SelectedCount > 0;

	public Folderer(string? title = null) : base(title) {
		Shared = UserProfilesFolderRepo.Connect()
		.Transform(i => {
			i.title ??= "All";
			return new ObsFolder(folder: i, selectedChanged: SelectedChanged);
		})
		.Publish()    // <-- multicast
		.RefCount();  // <-- auto-connect when first subscriber appears

		_ = Shared.SortAndBind(out var folders, AscendingComparer).Subscribe();
		Folders = folders;
	}
	public virtual void SelectedChanged(ObsFolder folder) {
		OnPropertyChanged(nameof(HasSelectedItems));
		OnPropertyChanged(nameof(SelectedCount));
	}
	public virtual ObsFolder Deleted(ObsFolder profile) {
		return profile;
	}
}
