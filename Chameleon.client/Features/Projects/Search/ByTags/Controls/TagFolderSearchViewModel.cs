using Chameleon.app.Avalonia;
using Chameleon.client.Features.Projects;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.ProfilesAndFolders.Search.ByTags.Controls;
public partial class TagFolderSearchViewModel : TagsSearchViewModelBase {

	private readonly ReadOnlyObservableCollection<ObsFolder> folders;
	public ReadOnlyObservableCollection<ObsFolder> Folders => folders;

	[ObservableProperty]
	private ObsFolder? selectedFolder;

	public TagFolderSearchViewModel(TagItemDto tagItem) : base(tagItem) {

		_ = UserProfilesFolderRepo
		.Connect()
		.Filter(f => tagItem.Ids.Any(id => id == f.id.ToString()))
		.Transform(i => new ObsFolder(i))
		.SortAndBind(out folders, FoldersViewModel.AscendingComparer)
		.Subscribe();

		_ = this.WhenValueChanged(x => x.SelectedFolder)
			.Where(folder => folder is not null)
			.Subscribe(folder => Navigator.NavigateToType(typeof(ProjectsView), folder));
	}
}
