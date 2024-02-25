using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Prism.Module.Auth.ViewModels;

namespace Chameleon.Avalonia.Prism.Module.Auth;

public partial class AuthView : UserControl
{
    public AuthView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Pass the parent window to the ViewModel
        // given that the ViewModel has been binded to this view
        AuthViewModel? viewModel = this.DataContext as AuthViewModel;
        if (this.Parent is Window parent &&
            viewModel is not null)
        {
            viewModel.ParentWindow = parent;
        }
    }
}