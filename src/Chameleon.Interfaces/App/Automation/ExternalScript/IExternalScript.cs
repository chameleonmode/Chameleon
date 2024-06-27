using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.ExternalScript;
public interface IExternalScript
{
    Task Run(IBrowserContext browserContext, IDictionary<string, string> parameters);
}
