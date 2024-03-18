using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.App.Settings;
using Chameleon.Interfaces.App.UserSettings.View;

namespace Chameleon.Avalonia.Controls.Settings;

public partial class PhoneVerificationView : UserControl,
    IPhoneVerificationView
{
    public PhoneVerificationView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IPhoneVerificationViewModel>();
    }
}