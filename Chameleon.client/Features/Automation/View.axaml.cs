using Avalonia.Controls.Primitives;
using FluentAvalonia.UI.Controls;
using Chameleon.client.Features.Automation.Actors;
using Chameleon.client.Features.Automation.Playwright;
using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Automation;

[lib.Common.Attributes.ViewModel(typeof(ViewModel))]
public partial class View : TabStripNavigationPage {
  public View() {
    InitializeComponent();
    SetEvents();
  }
  public override TabStrip Strip => ActiveTabStrip;
  public override Frame Frame => NavigationFrame;
  public override Type GetNavigationTarget(int index) => index switch {
    0 => typeof(ActorsView),
    1 => typeof(PlaywrightView),
    2 => typeof(AI.ChameleonAIR.View),
    _ => throw new Exception()
  };
}
