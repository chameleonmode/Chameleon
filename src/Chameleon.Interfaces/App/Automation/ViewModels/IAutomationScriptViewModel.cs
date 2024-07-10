using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface IAutomationScriptViewModel
    : ITransientDependency
    , IHaveInitialize
{
    int Id { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    string Filepath { get; set; }
    bool IsHasParameter { get; }
    IList<IAutomationScriptParameterViewModel> Parameters { get; set; }

    IAutomationScriptDescription ScriptDescription { get; }
}
