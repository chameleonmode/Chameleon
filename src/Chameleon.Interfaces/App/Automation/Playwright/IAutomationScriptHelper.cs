using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IAutomationScriptHelper
    : ISingletonDependency
{
    public BrowserTypeLaunchPersistentContextOptions CreateOptions(List<string> args, string exts, string browserExeFilePath, IProxySettings proxy);
    public Task InitScriptAsync(IBrowserContext browserContext);
}
