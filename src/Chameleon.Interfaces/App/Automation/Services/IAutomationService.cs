using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationService
    : ISingletonDependency
{
    Task<List<IAutomationScriptDescription>> GetAll();
    Task<List<IAutomationScriptDescription>> GetAll(string filepath);
    Task UpdateParameter(IAutomationScriptParameter param);
    Task SetParametersValue(IList<IAutomationParameterValue> values);
    Task<string> GetScriptBody(int id);
    Task<string> GetScriptBody(string filepath);
}
