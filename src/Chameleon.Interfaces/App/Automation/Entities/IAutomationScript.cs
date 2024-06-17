using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.App.Automation.Entities;
public interface IAutomationScript
    : IEntity
{
    string Title { get; set; }
    string Description { get; set; }
    IList<IAutomationScriptParameter> Parameters { get; set; }
}
