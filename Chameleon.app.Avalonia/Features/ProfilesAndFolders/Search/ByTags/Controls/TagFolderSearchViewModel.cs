using Chameleon.app.Avalonia.DynamicData;
using Chameleon.app.Avalonia.Models.Observable;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.app.Avalonia.Features.Search.ByTags.Controls;
public partial class TagFolderSearchViewModel : ObservableObject {
	public string Type { get; }

	private readonly ReadOnlyObservableCollection<ObsFolder> folders;
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;

	public TagFolderSearchViewModel(TagItemDto tagItem) {

		Type = tagItem.Type;

		_ = UserProfilesFolderRepo
		.Connect()
		.Filter(f => tagItem.Ids.Any(id => id == f.id.ToString()))
		.Transform(i => new ObsFolder(i))
		.SortAndBind(out folders, Compares.ObsFolderCompares.AscendingComparer)
		.Subscribe();
	}
}
