using Chameleon.Interfaces.WebBrowser;
using Microsoft.Playwright;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public class PlaywrightBrowserLaunchOptions
    : SystemBrowserLaunchOptions
    , IPlaywrightBrowserLaunchOptions
{
    public IPlaywright Playwright { get; set; }
}
