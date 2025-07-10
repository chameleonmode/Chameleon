using Avalonia.Controls.Primitives;
using Chameleon.client.Features.Projects.Folders;
using Chameleon.client.UI.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Settings.Featured;

[MvvM.ViewModel(typeof(FunctionalSettingsViewModel))]
public partial class FunctionalSettingsView : TabStripNavigationPage {
  public FunctionalSettingsView() {
    InitializeComponent();
  }
  public override TabStrip Strip => ActiveTabStrip;
  public override Frame Frame => NavigationFrame;
	public override Type GetNavigationTarget(int index) => index switch {
		0 => typeof(UserDefaultSettingsView),
		1 => typeof(PhoneVerificationView),
		2 => typeof(UserProxySettingsView),
		3 => typeof(ProxyCreditView),
		_ => throw new Exception()
	};
	public override void OnAfterNavigatedToViewModel(object param) {
		base.OnAfterNavigatedToViewModel(param);
		if (param is ObsFolder) ActiveTabStrip.SelectedIndex = 2;
	}
}