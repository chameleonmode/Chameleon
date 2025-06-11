using Avalonia;
using Chameleon.client.UI.Pages;

namespace Chameleon.client.Features.Projects.Profiles.Identity;

[MvvM.ViewModel(typeof(IdentityViewModel))]
public partial class IdentityView : ChameleonPageBase {
    public IdentityView() {
        InitializeComponent();
    }
    public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}