using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(PlaywrightViewModel))]
public partial class PlaywrightView : ChameleonNavigationPage {
    public PlaywrightView()
    {
        InitializeComponent();
    }
}
