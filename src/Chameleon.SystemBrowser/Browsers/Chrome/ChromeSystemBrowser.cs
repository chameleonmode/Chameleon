namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
            ISetPreferencesService setPreferencesService,
            IUserDefaultSettingsService userDefaultsSettingsService)
        : SystemBrowserBase(eventAggregator), IChromeSystemBrowser
    {

        public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
        {
            return new ChromeSystemBrowserInstance(
                EventAggregator,
                o,
                     setPreferencesService,
                 applicationEnvironment,
                 userDefaultsSettingsService,
                GetBrowserExePath());
        }

        protected string GetBrowserExePath()
        {
            return systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }
    }
}
