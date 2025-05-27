using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.client;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Views;

public partial class PhoneVerificationView : ChameleonPageBase
{
    public PhoneVerificationView()
    {
        InitializeComponent();

        ControlName = "Phone Verification";
        Description = "PVA key settings and API simplified";
        PreviewImage = App.TryGetResource<IconSource>("Phone");
    }
}