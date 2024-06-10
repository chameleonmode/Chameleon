using Chameleon.Playwright.Automation.Brave;
using Chameleon.Playwright.Automation.Chrome;
using Chameleon.Interfaces.App.Automation.Manager;
using Chameleon.Interfaces.App.Automation.Playwright;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Playwright.Automation.Manager;
public class PlaywrightBrowserManager : IPlaywrightBrowserManager
{
    private readonly Dictionary<SystemBrowserType, Type> _mapping =
        new Dictionary<SystemBrowserType, Type>()
        {
            { SystemBrowserType.Chrome, typeof(IChromePlaywrightBrowser) },
            { SystemBrowserType.Brave, typeof(IBravePlaywrightBrowser) },
        };
    private readonly IHaveContainerProvider _containerProvider;

    public PlaywrightBrowserManager(IHaveContainerProvider containerProvider)
    {
        _containerProvider = containerProvider;
    }

    public IPlaywrightBrowser Get(SystemBrowserType browserType)
    {
        if (_mapping.TryGetValue(browserType, out var type))
        {
            return (IPlaywrightBrowser)_containerProvider.Resolve(type);
        }

        throw new KeyNotFoundException(browserType.ToString());
    }
}
