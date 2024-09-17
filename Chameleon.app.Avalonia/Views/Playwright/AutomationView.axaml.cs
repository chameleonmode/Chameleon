using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Automation.Views;
using Chameleon.app.Avalonia.ViewModels.Playwright;

namespace Chameleon.app.Avalonia.Views.Playwright;

[ViewModel(typeof(AutomationViewModel))]
public partial class AutomationView
    : ViewControlBase<AutomationViewModel>
    , IAutomationView
{
    public AutomationView()
    {
        InitializeComponent();
    }
}
