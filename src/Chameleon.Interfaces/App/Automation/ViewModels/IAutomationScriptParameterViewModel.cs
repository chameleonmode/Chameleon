using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface IAutomationScriptParameterViewModel
    : ITransientDependency
{
    int Id { get; }
    string Name { get; }
    string Value { get; set; }
}
