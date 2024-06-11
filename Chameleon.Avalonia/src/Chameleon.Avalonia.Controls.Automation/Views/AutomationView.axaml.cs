using Chameleon.Avalonia.Controls.Automation.ViewModels;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Core.Attributes;
using Chameleon.Interfaces.App.Automation.Views;

namespace Chameleon.Avalonia.Controls.Automation.Views;

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
