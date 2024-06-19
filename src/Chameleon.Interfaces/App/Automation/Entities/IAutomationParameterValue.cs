using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.App.Automation.Entities;
public interface IAutomationParameterValue
    : IEntity
{
    int ParameterId { get; set; }
    string Name { get; set; }
    string Value { get; set; }
}
