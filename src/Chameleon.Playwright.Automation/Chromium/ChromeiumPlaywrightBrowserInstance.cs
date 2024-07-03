
namespace Chameleon.Playwright.Automation.Chrome;
public class ChromeiumPlaywrightBrowserInstance(IPlaywrightBrowserLaunchOptions options)
    : IPlaywrightBrowserInstance
{
    private IBrowser _browser;

    public IBrowserContext BrowserContext => _browser?.Contexts?[0];

    public async Task Open()
    {
        _browser = await options.Playwright.Chromium.ConnectOverCDPAsync($"http://localhost:{options.UserProfileVM.SBI.Port}");
    }
}
