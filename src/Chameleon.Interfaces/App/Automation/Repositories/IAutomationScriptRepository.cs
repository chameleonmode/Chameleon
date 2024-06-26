using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.App.Automation.Repositories;
public interface IAutomationScriptRepository
    : IRepository<IAutomationScript>
{
    void UpdateParameter(IAutomationScriptParameter param);
    void SetParametersValue(IList<IAutomationParameterValue> values);
    IList<IAutomationScriptDescription> GetAllScriptDescription();
    string GetScriptBody(int id);
}
