

namespace Chameleon.Playwright.Automation.Chrome;
public class ChromeiumPlaywrightBrowserInstance(IPlaywrightBrowserLaunchOptions options)
    : IPlaywrightBrowserInstance
{
    private IBrowser _browser;

    public IBrowserContext BrowserContext => _browser?.Contexts?[0];

    public  async Task Close()
    {
        if (BrowserContext != null)
        {
            await BrowserContext.CloseAsync();
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }
    }


    public Task Open()
        => TryOpenByCDP(0);

    private async Task TryOpenByCDP(int v)
    {
        try
        {
            _browser = await options.Playwright.Chromium.ConnectOverCDPAsync($"http://localhost:{options.UserProfileVM.SBI.Port}");
        }
        catch
        {
            if (v < 6)
            {
                await Task.Delay(1000);
                await TryOpenByCDP(v + 1);
            }
            else
            {
                throw;
            }
        }
    }

    public async Task Record()
    {
        var page = await BrowserContext.NewPageAsync();
        await page?.PauseAsync();
    }
}
