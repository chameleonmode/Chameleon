using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Manager;
public interface IPlaywrightBrowserManager : ISingletonDependency
{
    IPlaywrightBrowser Get(SystemBrowserType browserType);
}
