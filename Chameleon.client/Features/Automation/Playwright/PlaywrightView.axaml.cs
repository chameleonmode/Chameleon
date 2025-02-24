using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Features.Automation.Playwright;
public partial class PlaywrightView : ChameleonPageBase {
    public PlaywrightView()
    {
        InitializeComponent();
		ControlName = "Playwright Asisstant";
		Description = "Configure your Browser profiles playwright scripts settings";
		PreviewImage = App.TryGetResource<IconSource>("Playwright");
    }
}
