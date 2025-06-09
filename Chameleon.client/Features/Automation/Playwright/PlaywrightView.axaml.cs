using Chameleon.client.UI.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Automation.Playwright;
public partial class PlaywrightView : ChameleonPageBase {
    public PlaywrightView()
    {
        InitializeComponent();
		ControlName = "Automations";
		Description = "Configure your Browser profiles automation scripts settings";
		PreviewImage = App.TryGetResource<IconSource>("Playwright");
    }
}
