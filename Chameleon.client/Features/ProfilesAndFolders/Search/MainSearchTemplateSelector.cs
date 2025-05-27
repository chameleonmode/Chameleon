using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace Chameleon.client.Features.ProfilesAndFolders.Search;
public class MainSearchTemplateSelector : IDataTemplate {

	[Content]
	public Dictionary<string, IDataTemplate> Templates { get; } = [];

	public Control? Build(object? param) {
		return param is null ? null : Templates[((MainAppSearchItem)param).SearchType].Build(param);
	}
	public bool Match(object? data) {
		return data is not null && data is MainAppSearchItem;
	}
}
