using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(PlaywrightViewModel))]
public partial class PlaywrightView
		: ViewControlBase<PlaywrightViewModel>
{
    public PlaywrightView()
    {
        InitializeComponent();
    }
}
