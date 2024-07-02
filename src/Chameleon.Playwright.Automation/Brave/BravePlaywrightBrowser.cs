namespace Chameleon.Playwright.Automation.Brave;
public class BravePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService)
        : BraveSystemBrowser(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService),
    IBravePlaywrightBrowser
{
    public virtual async Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions o)
    {
        BravePlaywrightBrowserInstance browser = null;
        try
        {
            browser = new BravePlaywrightBrowserInstance(
            EventAggregator,
            o,
            SetPreferencesService,
            ApplicationEnvironment,
            UserDefaultSettingsService,
            GetBrowserExePath());

            await browser.Open();
        }
        catch (Exception e)
        {
            await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
        }

        return browser;
    }

    // use BraveSystemBrowser for call this method
    public override Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
