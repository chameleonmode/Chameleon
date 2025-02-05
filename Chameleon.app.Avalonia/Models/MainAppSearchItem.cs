namespace Chameleon.app.Avalonia.Models;

public class SearchType {
	public const string Default = nameof(Default);
	public const string Tags = nameof(Tags);
}

public class MainAppSearchItem {
	public MainAppSearchItem() { }

	public MainAppSearchItem(string pageHeader, Type pageType)
	{
		Header = pageHeader;
		PageType = pageType;
	}

	public string? Header { get; set; }
							 
	public object? ViewModel { get; set; }
							 
	public string? Namespace { get; set; }

	public Type? PageType { get; set; }

	public string SearchType { get; set; } = Models.SearchType.Default;

	public object? Value { get; set; } 
}