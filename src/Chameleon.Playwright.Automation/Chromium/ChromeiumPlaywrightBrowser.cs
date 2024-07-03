namespace Chameleon.Playwright.Automation.Chrome;
public class ChromeiumPlaywrightBrowser
    : IChromeiumPlaywrightBrowser
{
    public virtual async Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions o)
    {
        var browser  = new ChromeiumPlaywrightBrowserInstance(o);
        await browser.Open();
        return browser;
    }
}
