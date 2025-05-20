using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Search.ByTags;
public class TagsSearchViewModelBase : ObservableObject {
	public string Type { get; }
	public TagsSearchViewModelBase(TagItemDto tagItem) {

		Type = tagItem.Type;
	}
}
