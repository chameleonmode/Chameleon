namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
            ISetPreferencesService setPreferencesService,
            IUserDefaultSettingsService userDefaultsSettingsService)
    : SystemBrowserBase(eventAggregator, applicationEnvironment, setPreferencesService, userDefaultsSettingsService),
        IChromeSystemBrowser
    {

        public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
        {
            return new ChromeSystemBrowserInstance(
                EventAggregator,
                o,
                SetPreferencesService,
                ApplicationEnvironment,
                UserDefaultSettingsService
                );
        }

        protected override string GetBrowserExePath()
        {
            return systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }

        protected override string GetSystemBrowserExePath() =>
            GetBrowserExePath();

        protected override string GetDirectoryPath()
        {
            throw new NotImplementedException();
        }
    }
}
