using Chameleon.Interfaces.App.Automation.Playwright;
using Microsoft.Playwright;

namespace Chameleon.Playwright.Automation;
public interface IPlaywrightBrowserInstanceWithContext
    : IPlaywrightBrowserInstance
{
    IBrowserContext BrowserContext { get; }
}
