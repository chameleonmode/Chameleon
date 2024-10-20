namespace Chameleon.app.Avalonia.Models;
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
}