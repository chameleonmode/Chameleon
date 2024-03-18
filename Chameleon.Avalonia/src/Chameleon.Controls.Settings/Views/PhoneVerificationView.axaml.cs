using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.App.UserSettings.View;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class PhoneVerificationView : SubPageViewControl,
    IPhoneVerificationView
{
    public PhoneVerificationView()
    {
        InitializeComponent();
    }
}