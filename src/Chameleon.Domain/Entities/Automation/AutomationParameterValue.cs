using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.Automation.ViewModels;

namespace Chameleon.Domain.Entities.Automation;
public class AutomationParameterValue
    : IAutomationParameterValue
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int ParameterId { get; set; }

    public AutomationParameterValue()
    {
    }

    public AutomationParameterValue(IAutomationScriptParameterViewModel scriptParameterViewModel)
    {
        ParameterId = scriptParameterViewModel.Id;
        Name = scriptParameterViewModel.Name;
        Value = scriptParameterViewModel.Value;
    }
}
