using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(PlaywrightViewModel))]
public partial class PlaywrightView
		: AutoViewModelLocatorControl {
    public PlaywrightView()
    {
        InitializeComponent();
    }
}
