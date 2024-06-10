using Chameleon.Interfaces.WebBrowser;
using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IPlaywrightBrowserLaunchOptions 
    : ISystemBrowserLaunchOptions
{
    IPlaywright Playwright { get; }
}
