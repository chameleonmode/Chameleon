using Chameleon.Interfaces.App.Automation.Playwright;
using Microsoft.Playwright;

namespace Chameleon.Avalonia.Playwright.Automation;
public interface IPlaywrightBrowserInstanceWithContext
    : IPlaywrightBrowserInstance
{
    IBrowserContext BrowserContext { get; }
}
