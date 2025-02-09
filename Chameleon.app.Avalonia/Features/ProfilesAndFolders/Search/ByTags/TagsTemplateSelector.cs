using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Chameleon.app.Avalonia.Features.Search.ByTags.Controls;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.Features.Search.ByTags;
public class TagsTemplateSelector : IDataTemplate {

	[Content]
	public Dictionary<string, IDataTemplate> Templates { get; } = [];

	public Control? Build(object? param) {
		return param switch {
			TagFolderSearchViewModel { Type: TagItemType.Folder } => Templates[TagItemType.Folder].Build(param),
			TagProfilesSearchViewModel { Type: TagItemType.Profile } => Templates[TagItemType.Profile].Build(param),
			_ => null
		};
	}

	public bool Match(object? data) {
		return data is not null && (data is TagFolderSearchViewModel || data is TagProfilesSearchViewModel);
	}
}
