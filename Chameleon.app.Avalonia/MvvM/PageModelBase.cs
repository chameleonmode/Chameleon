using Avalonia.Controls;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.MvvM;

public class PageModelBase {
	public Type? Tag { get; set; }
	public string? NavHeader { get; set; }
	public string IconKey { get; set; } = "HomeIcon"; // Default to "HomeIcon
	public bool ShowsInFooter { get; set; }

	public NavigationViewItemBase GetNavigationViewItemBase(UserControl c)
	{
		var nvi = new NavigationViewItem {
			Content = NavHeader,
			Tag = this,
			IconSource = (IconSource)c.FindResource(IconKey)!,
		};
		nvi.Classes.Add("MainAppNav");
		return nvi;
	}
}
