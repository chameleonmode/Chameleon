using Chameleon.Interfaces.App.Automation.Entities;

namespace Chameleon.Domain.Entities.Automation;
public class AutomationParameterValue
    : IAutomationParameterValue
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int ParameterId { get; set; }
}
