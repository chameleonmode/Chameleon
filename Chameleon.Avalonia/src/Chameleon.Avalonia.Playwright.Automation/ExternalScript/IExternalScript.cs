using Chameleon.Interfaces.WebBrowser;
using Microsoft.Playwright;

namespace Chameleon.Avalonia.Playwright.Automation.ExternalScript;
public interface IExternalScript
{
    SystemBrowserType BrowserType { get; }

    Task Run(IBrowserContext browserContext, IDictionary<string, string> parameters);
}
