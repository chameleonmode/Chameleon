namespace Chameleon.Infrastructure.App.Automation;
public class AutomationService(IAutomationScriptRepository repository)
    : IAutomationService
{
    private IList<IAutomationScriptDescription> _scripts;
    private Task<List<IAutomationScriptDescription>> ThesesScripts => Task.Run(
    () =>
    {
        var entities = repository.GetAllScriptDescription();
        var response = new List<IAutomationScriptDescription>(entities);

        return response;
    });


    public async Task<IList<IAutomationScriptDescription>> GetAll()
    {
        return await ThesesScripts;
    }

    public void UpdateParameter(IAutomationScriptParameter param)
    {
        repository.UpdateParameter(param);
    }

    public void SetParametersValue(IList<IAutomationParameterValue> values)
    {
        repository.SetParametersValue(values);
    }

    public Task<string> GetScriptBody(int id)
        => Task.Run(() => repository.GetScriptBody(id));
}
