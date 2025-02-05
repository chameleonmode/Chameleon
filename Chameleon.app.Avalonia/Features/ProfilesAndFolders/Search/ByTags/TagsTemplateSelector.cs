using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.app.Avalonia.Features.Search.ByTags;
public class TagsTemplateSelector : IDataTemplate {

	[Content]
	public Dictionary<string, IDataTemplate> Templates { get; } = [];

	public Control? Build(object? param) {
		return param is null ? null : Templates[((TagItemDto)param).Type].Build(param);
	}

	public bool Match(object? data) {
		return data is not null && data is TagItemDto;
	}
}
