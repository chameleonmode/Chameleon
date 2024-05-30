using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.ViewModels;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels;
public class AutomationScriptParameterViewModel 
    : PageViewModelBase
    , IAutomationScriptParameterViewModel
{
    private readonly IAutomationParameterValue _automationParameterValue;

    public AutomationScriptParameterViewModel(IAutomationParameterValue automationParameterValue)
    {
        _automationParameterValue = automationParameterValue;
        Value = automationParameterValue.Value;
    }

    public int Id => _automationParameterValue.Id;

    public string Name => _automationParameterValue.Name;

    private string _value;
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
