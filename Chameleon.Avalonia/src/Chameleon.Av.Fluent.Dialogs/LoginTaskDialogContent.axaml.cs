using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class LoginTaskDialogContent : UserControl
{
    public LoginTaskDialogContent()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Current.ContainerProvider?.Resolve<IAuthTaskDialogViewModel>();
    }
}