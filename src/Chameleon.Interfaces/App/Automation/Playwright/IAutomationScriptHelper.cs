using Chameleon.Interfaces.Ioc;
using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IAutomationScriptHelper
    : ISingletonDependency
{
    public BrowserTypeLaunchPersistentContextOptions CreateOptions(List<string> args, string exts, string browserExeFilePath);
    public Task InitScriptAsync(IBrowserContext browserContext);
}
