using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers;
using Chameleon.SystemBrowser.Firefox;

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

        private string GetBrowserExePath()
        {
            return systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }
    }
}
