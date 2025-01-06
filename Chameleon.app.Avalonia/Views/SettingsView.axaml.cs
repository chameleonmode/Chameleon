using Avalonia.Interactivity;

using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(SettingsViewModel))]
public partial class SettingsView : ChameleonNavigationPage {
	public SettingsView()
	{
		InitializeComponent();
		LaunchSupportLinkItem.Click += LaunchSupportLinkItemClick;
	}

	private void LaunchSupportLinkItemClick(object? sender, RoutedEventArgs e)
	{
		var uri = new Uri("https://github.com/chameleonmode/chameleon.app-CommunityPipeline");
		try {
			ProUtil.GoToUrlDefault(uri.ToString());
		} catch {
			Toaster.Error($"Error navigationg to {uri}");
		}
	}
}