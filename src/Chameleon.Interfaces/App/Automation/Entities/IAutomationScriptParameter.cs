using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.App.Automation.Entities;
public interface IAutomationScriptParameter
    : IEntity
{
    string Name { get; set; }
    int ScriptId { get; set; }
    IAutomationParameterValue Value { get; set; }
}
