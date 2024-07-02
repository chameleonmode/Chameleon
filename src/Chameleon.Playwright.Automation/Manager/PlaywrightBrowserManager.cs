namespace Chameleon.Playwright.Automation.Manager;
public class PlaywrightBrowserManager(IHaveContainerProvider containerProvider) : IPlaywrightBrowserManager
{
    private readonly Dictionary<SystemBrowserType, Type> _mapping =
        new Dictionary<SystemBrowserType, Type>()
        {
            { SystemBrowserType.Chrome, typeof(IChromePlaywrightBrowser) },
            { SystemBrowserType.Brave, typeof(IBravePlaywrightBrowser) },
        };
    public IPlaywrightBrowser Get(SystemBrowserType browserType)
    {
        if (_mapping.TryGetValue(browserType, out var type))
        {
            return (IPlaywrightBrowser)containerProvider.Resolve(type);
        }

        throw new KeyNotFoundException(browserType.ToString());
    }
}
