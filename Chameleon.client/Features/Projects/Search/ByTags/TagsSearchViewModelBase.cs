using Chameleon.lib.Abs.Repos;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Projects.Search.ByTags;
public class TagsSearchViewModelBase(TagItemDto tagItem) : ObservableObject {
	public string Type { get; } = tagItem.Type;
}
