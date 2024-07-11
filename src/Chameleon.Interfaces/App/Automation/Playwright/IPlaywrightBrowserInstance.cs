using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IPlaywrightBrowserInstance
{
    IBrowserContext BrowserContext { get; }
    Task Open();
    Task Close();
    Task Record();
}
