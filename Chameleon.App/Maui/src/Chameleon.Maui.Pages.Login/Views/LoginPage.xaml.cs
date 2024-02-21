using Chameleon.Maui.Pages.Login.ViewModels;
using Chameleon.Maui.Toolkit.Base;

namespace Chameleon.Maui.Pages.Login.Views;

public partial class LoginPage : BasePage<LoginPageViewModel>
{
    public LoginPage(LoginPageViewModel vm)
        : base(vm)
    {
        InitializeComponent();
    }
}