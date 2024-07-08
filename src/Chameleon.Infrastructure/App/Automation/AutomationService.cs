using Chameleon.Core.Util;
using Chameleon.Domain.Entities.Automation;
using System.IO;

namespace Chameleon.Infrastructure.App.Automation;
public class AutomationService(IAutomationScriptRepository repository)
    : IAutomationService
{
    public Task<List<IAutomationScriptDescription>> GetAll() => Task.Run(() =>
    {
        var entities = repository.GetAllScriptDescription();
        var response = new List<IAutomationScriptDescription>(entities);

        return response;
    });

    public Task<List<IAutomationScriptDescription>> GetAll(string filepath) => Task.Run(() =>
    {
        var returned = new List<IAutomationScriptDescription>();
        foreach (var item in IOtil.ReadDirectory(filepath))
        {
            FileInfo inf = new FileInfo(item);
            returned.Add(new AutomationScriptDescription()
            {
                Id = -1,
                Title = inf.Name,
                Description = inf.Directory.Name,
                FilePath = inf.FullName,
            });
        }
        return returned;
    });

    public Task UpdateParameter(IAutomationScriptParameter param) => Task.Run(() =>
        repository.UpdateParameter(param));


    public Task SetParametersValue(IList<IAutomationParameterValue> values) => Task.Run(() => 
        repository.SetParametersValue(values));

    public Task<string> GetScriptBody(int id) => Task.Run(() =>
        repository.GetScriptBody(id));

    public Task<string> GetScriptBody(string filepath) => File.Exists(filepath) ? File.ReadAllTextAsync(filepath) : Task.FromResult(string.Empty);
}
