using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers;
using Chameleon.SystemBrowser.Firefox;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowser : SystemBrowserBase, IChromeSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        private readonly ISetPreferencesService _setPreferencesService;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

        public ChromeSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
            ISetPreferencesService setPreferencesService,
             IUserDefaultSettingsService userDefaultsSettingsService
            )
        {
            _eventAggregator = eventAggregator;
            _applicationEnvironment = applicationEnvironment;
            _systemBrowserInfoManager = systemBrowserInfoManager;
            _setPreferencesService = setPreferencesService;
            _userDefaultsSettingsService = userDefaultsSettingsService;
        }

        public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
        {
            return new ChromeSystemBrowserInstance(
                _eventAggregator,
                o,
                     _setPreferencesService,
                 _applicationEnvironment,
                 _userDefaultsSettingsService,
                GetBrowserExePath());
        }

        private string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }
    }
}
