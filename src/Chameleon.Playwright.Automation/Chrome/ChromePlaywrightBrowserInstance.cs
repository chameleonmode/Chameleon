namespace Chameleon.Playwright.Automation.Chrome;
public class ChromePlaywrightBrowserInstance(
    IEventAggregator eventAggregator,
    IPlaywrightBrowserLaunchOptions options,
    ISetPreferencesService setPreferencesService,
    IApplicationEnvironment applicationEnvironment,
    IUserDefaultSettingsService userDefaultsSettingsService,
    string browserExeFilePath)
    : ChromeSystemBrowserInstance(
        eventAggregator,
        options,
        setPreferencesService,
        applicationEnvironment,
        userDefaultsSettingsService,
        browserExeFilePath),
    IPlaywrightBrowserInstance
{
    private IBrowser _browser;
    public IBrowserContext BrowserContext => _browser.Contexts[0];

    public Task Close()
    {
        throw new NotImplementedException();
    }


    public override async Task Open()
    {
        _browser = await options.Playwright.Chromium.ConnectOverCDPAsync($"http://localhost:{options.UserProfileVM.SBI.Port}");
    }
}
