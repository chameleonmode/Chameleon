using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Auth;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class LoginTaskDialog : UserControl, ILoginTaskDialog
{
    public LoginTaskDialog()
    {
        InitializeComponent();
    }
}