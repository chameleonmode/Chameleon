using Chameleon.Interfaces.App.Automation.Entities;

namespace Chameleon.Domain.Entities.Automation;
public class AutomationScriptDescription
     : ObservableEntities<IAutomationScriptDescription>
     , IAutomationScriptDescription
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Script { get; set; }

    public IList<IAutomationParameterValue> Parameters { get; set; } = new List<IAutomationParameterValue>();
}
