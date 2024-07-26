using Avalonia;

namespace Chameleon.Avalonia.Controls.Settings.Functional.Views;

public partial class PhoneVerificationView : SubPageViewControl,
    IPhoneVerificationView
{
    public PhoneVerificationView()
    {
        InitializeComponent();

        ControlName = "Phone Verification";
        Description = "PVA key settings and API simplified";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("CellPhone");
    }
}