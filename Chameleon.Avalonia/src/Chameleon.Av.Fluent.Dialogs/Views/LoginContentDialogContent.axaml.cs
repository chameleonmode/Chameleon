using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Dialogs.Controls;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class LoginContentDialogContent : ContentDialogControlBase, ILoginContentDialogContent
{
    public LoginContentDialogContent()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Current.ContainerProvider?.Resolve<IAuthTaskDialogViewModel>();
    }
    public override string PrimaryButtonText  => "Login";       
}