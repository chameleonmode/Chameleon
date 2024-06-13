using Chameleon.Interfaces.App.Automation.Entities;

namespace Chameleon.Domain.Entities.Automation;
public class AutomationScript
     : IAutomationScript
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Script { get; set; }

    public IList<IAutomationScriptParameter> Parameters { get; set; } = new List<IAutomationScriptParameter>();
}
