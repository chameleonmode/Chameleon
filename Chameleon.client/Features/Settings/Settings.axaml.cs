using Avalonia.Interactivity;
using Chameleon.client.UI.Pages;
using Chameleon.lib.Helpers;
using Chameleon.lib.Util;

namespace Chameleon.client.Features.Settings;

[MvvM.ViewModel(typeof(ViewModel))]
public partial class View : ChameleonNavigationPage {
	public View()
	{
		InitializeComponent();
		LaunchSupportLinkItem.Click += LaunchSupportLinkItemClick;
	}

	private void LaunchSupportLinkItemClick(object? sender, RoutedEventArgs e)
	{
		var uri = new Uri("https://github.com/chameleonmode/chameleon.app-CommunityPipeline");
		try {
			Processez.OpenBrowser(uri.ToString());
		} catch {
			Toaster.Error($"Error navigationg to {uri}");
		}
	}
}