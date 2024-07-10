namespace Chameleon.Playwright.Automation.Chrome;
public class ChromePlaywrightBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        ISetPreferencesService setPreferencesService,
        IUserDefaultSettingsService userDefaultsSettingsService)
    : ChromeSystemBrowser(eventAggregator,
            applicationEnvironment,
            systemBrowserInfoManager,
            setPreferencesService,
            userDefaultsSettingsService), 
    IChromePlaywrightBrowser
{
    public virtual Task<IPlaywrightBrowserInstance> Open(IPlaywrightBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
        // ChromePlaywrightBrowserInstance browser = null;
        // try
        // {
        //     browser = new ChromePlaywrightBrowserInstance(
        //     EventAggregator,
        //     o,
        //     SetPreferencesService,
        //     ApplicationEnvironment,
        //     UserDefaultSettingsService,
        //     GetBrowserExePath());

        //     await browser.Open();
        // }
        // catch (Exception e)
        // {
        //     await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
        // }

        // return browser;
    }

    // use ChromeSystemBrowser for call this method
    public override Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        throw new NotImplementedException();
    }
}
