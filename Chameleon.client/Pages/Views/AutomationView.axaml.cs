using Avalonia.Controls.Primitives;
using Chameleon.client.Features.Automation.Actors;
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
		0 => typeof(ActorsView),
		1 => typeof(PlaywrightView),
		2 => typeof(ChameleonAIRView),
		_ => throw new Exception()
	};
}