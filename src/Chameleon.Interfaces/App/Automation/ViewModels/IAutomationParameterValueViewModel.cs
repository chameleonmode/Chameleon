using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.ViewModels;
public interface IAutomationParameterValueViewModel
    : ISingletonDependency
{
    int Id { get; }
    int UserId { get; }
    string Value { get; }
    int ParameterId { get; }
}
