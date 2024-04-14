using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Dialogs.Controls;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class LoginContentDialogContent : ContentDialogControlBase, ILoginContentDialogContent
{
    public override object? Title => "User Login";//ContainerServiceHelper.Current.ContainerProvider?.Resolve<IDefaultContentDialogTitle>();
    public override string PrimaryButtonText  => "Login";
    
    public LoginContentDialogContent()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IAuthTaskDialogViewModel>();
    }     
}