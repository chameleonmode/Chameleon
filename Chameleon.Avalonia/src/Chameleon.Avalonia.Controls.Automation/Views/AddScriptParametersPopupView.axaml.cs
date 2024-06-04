using Chameleon.Avalonia.Controls.Automation.Views.ViewModels;
using Chameleon.Avalonia.Fluent.Common.Controls;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Automation.Views;

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
}