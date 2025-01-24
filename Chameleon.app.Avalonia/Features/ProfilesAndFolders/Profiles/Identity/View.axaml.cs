using Avalonia;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Features.ProfilesAndFolders.Profiles.Identity;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ViewModel))]
public partial class View : ChameleonPageBase {
    public View()
    {
        InitializeComponent();
    }
	public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}