namespace Chameleon.Playwright.Automation.Manager;
public class PlaywrightBrowserManager(IHaveContainerProvider containerProvider) 
    : IPlaywrightBrowserManager
{
    public IPlaywrightBrowser Get(SystemBrowserType browserType) => browserType switch
    {
        SystemBrowserType.Chrome or
        SystemBrowserType.Brave => (IPlaywrightBrowser)containerProvider.Resolve(typeof(IChromeiumPlaywrightBrowser)), 
        SystemBrowserType.Unknown => throw new NotImplementedException(),
        SystemBrowserType.Firefox => throw new NotImplementedException(),
        _ => throw new NotImplementedException(),
    };
}
