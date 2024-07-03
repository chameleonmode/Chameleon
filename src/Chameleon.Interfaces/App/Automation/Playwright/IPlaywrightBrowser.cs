using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IPlaywrightBrowser  : ISingletonDependency
{
    Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions options);
}
