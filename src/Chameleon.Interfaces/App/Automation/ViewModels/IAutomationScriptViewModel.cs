using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface IAutomationScriptViewModel
    : ITransientDependency
    , IHaveInitialize
{
    int Id { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    string ScriptBody { get; set; }
    bool IsHasParameter { get; }
    IList<IAutomationScriptParameterViewModel> Parameters { get; set; }
}
