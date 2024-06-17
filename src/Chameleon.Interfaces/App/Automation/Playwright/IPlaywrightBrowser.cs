using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IPlaywrightBrowser
{
    Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions options);
}
