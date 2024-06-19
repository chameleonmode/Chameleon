using Chameleon.Interfaces.App.Automation.Entities;

namespace Chameleon.Domain.Entities.Automation;
public class AutomationScriptParameter
    : IAutomationScriptParameter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ScriptId { get; set; }
    public IAutomationParameterValue Value { get; set; }
}
