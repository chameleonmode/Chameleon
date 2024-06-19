using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.Automation.ViewModels;

namespace Chameleon.Avalonia.Controls.Automation.ViewModels;
public class AutomationParameterValueViewModel
    : PageViewModelBase
    , IAutomationParameterValueViewModel
{
    private readonly IAutomationParameterValueViewModel _automationParameterValue;
    public AutomationParameterValueViewModel(IAutomationParameterValueViewModel automationParameterValue)
    {
        _automationParameterValue = automationParameterValue;
        Value = automationParameterValue.Value;
    }

    public int Id => _automationParameterValue.Id;

    public int ParameterId => _automationParameterValue.ParameterId;

    public int UserId => _automationParameterValue.UserId;

    private string _value;
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
