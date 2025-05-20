using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace Chameleon.client.Features.Search;
public class MainSearchTemplateSelector : IDataTemplate {

	[Content]
	public Dictionary<string, IDataTemplate> Templates { get; } = [];

	public Control? Build(object? param) {
		return param is null ? null : Templates[((MainAppSearchItem)param).SearchType].Build(param);
	}
	public bool Match(object? data) {
		return data is not null and MainAppSearchItem;
	}
}
