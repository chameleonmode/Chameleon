using Avalonia.Controls;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Domain.Entities.Automation;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;

namespace Chameleon.Avalonia.Controls.Automation.Views;

public partial class AddScriptParametersPopupView
    : AutoViewModelLocatorControl
    , IAddScriptParametersPopupView
{
    public AddScriptParametersPopupView()
    {
        InitializeComponent();
    }
}