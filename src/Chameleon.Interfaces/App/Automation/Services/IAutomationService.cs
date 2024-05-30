using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationService
    : ISingletonDependency
{
    IList<IAutomationScriptDescription> GetAll();
    void UpdateParameter(IAutomationScriptParameter param);
    void SetParametersValue(IList<IAutomationParameterValue> values);
}
