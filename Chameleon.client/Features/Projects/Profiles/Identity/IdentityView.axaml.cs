using Avalonia;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(IdentityViewModel))]
public partial class IdentityView : ChameleonPageBase {
    public IdentityView()
    {
        InitializeComponent();
    }
	public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}