using Chameleon.Interfaces.Entities;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.App.Automation.Entities;
public interface IAutomationScriptDescription
    : IEntity
    , IReadOnlyList<IAutomationScriptDescription>
    , INotifyCollectionChanged
{
    string Title { get; set; }
    string Description { get; set; }
    IList<IAutomationParameterValue> Parameters { get; set; }
}
