using Chameleon.lib.Common.Enums;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
            IUserDefaultSettingsService userDefaultsSettingsService)
    : SystemBrowserBase(eventAggregator, applicationEnvironment, userDefaultsSettingsService),
        IChromeSystemBrowser
    {
        public override SystemBrowserType BrowserType => SystemBrowserType.Chrome;

        public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
        {
            return new ChromeSystemBrowserInstance(
                EventAggregator,
                o,
                ApplicationEnvironment,
                UserDefaultSettingsService,
                GetBrowserExePath());
        }

        protected override string GetBrowserExePath()
        {
            return systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }

        protected override string GetSystemBrowserExePath()
        {
            throw new NotImplementedException();
        }

        protected override string GetDirectoryPath()
        {
            throw new NotImplementedException();
        }
    }
}
