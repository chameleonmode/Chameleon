using Chameleon.Avalonia.Controls.Automation.Views.ViewModels;
using Chameleon.Avalonia.Fluent.Common.Controls;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Automation.Views;
using Avalonia.Input;

namespace Chameleon.Avalonia.Controls.Automation.Views;

[ViewModel(typeof(AddScriptParametersPopupViewModel))]
public partial class AddScriptParametersPopupView
    : ViewControlBase<AddScriptParametersPopupViewModel>
    , IAddScriptParametersPopupView
{
    public AddScriptParametersPopupView()
    {
        InitializeComponent();
    }

    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left || e.Key == Key.Right)
        {
            e.Handled = true;
        }
        else if (e.Key == Key.Up || e.Key == Key.Down)
        {
            e.Handled = true;
        }
    }
}