using Avalonia.Controls.Primitives;
using Chameleon.client.Features.Automation.AI.ChameleonAIR;
using Chameleon.client.Features.Automation.Playwright;
using Chameleon.client.FluentUI.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Pages.Views;
public partial class AutomationView : TabStripNavigationPage {
	public AutomationView() {
		InitializeComponent();
		SetEvents();
	}
	public override TabStrip Strip => ActiveTabStrip;
	public override Frame Frame => NavigationFrame;
	public override Type GetNavigationTarget(int index) => index switch {
		0 => typeof(PlaywrightView),
		1 => typeof(ChameleonAIRView),
		_ => throw new Exception()
	};
}