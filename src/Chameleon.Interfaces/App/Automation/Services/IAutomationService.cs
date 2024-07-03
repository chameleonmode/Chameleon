using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationService
    : ISingletonDependency
{
    Task<IList<IAutomationScriptDescription>> GetAll();
    Task<IList<IAutomationScriptDescription>> GetAll(string filepath);
    void UpdateParameter(IAutomationScriptParameter param);
    void SetParametersValue(IList<IAutomationParameterValue> values);
    Task<string> GetScriptBody(int id);
}
