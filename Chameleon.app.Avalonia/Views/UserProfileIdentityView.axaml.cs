using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(UserProfileIdentityViewModel))]
public partial class UserProfileIdentityView : ChameleonPageBase {
    public UserProfileIdentityView()
    {
        InitializeComponent();
    }

	public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}