using Chameleon.Core.Util;
using Chameleon.Domain.Entities.Automation;
using System.IO;

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

    public async Task<IList<IAutomationScriptDescription>> GetAll(string filepath)
    {
        var returned = new List<IAutomationScriptDescription>();
        foreach (var item in await IOtil.ReadDirectory(filepath))
        {
            FileInfo inf = new FileInfo(item);
            returned.Add(new AutomationScriptDescription()
            {
                Title = inf.Name,
                Description = inf.Directory.Name
            });
        }

        return returned;
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
