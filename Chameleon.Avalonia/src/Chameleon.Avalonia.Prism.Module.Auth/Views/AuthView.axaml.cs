using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Avalonia.Prism.Module.Auth.ViewModels;
using Chameleon.Interfaces.Auth;

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
        //AuthTaskDialogViewModel? viewModel = this.DataContext as AuthTaskDialogViewModel;
        //if (this.Parent is Window parent &&
        //    viewModel is not null)
        //{
        //    viewModel.ParentWindow = parent;
        //}
    }
}