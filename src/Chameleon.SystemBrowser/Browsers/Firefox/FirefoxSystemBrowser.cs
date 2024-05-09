using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Browsers;

namespace Chameleon.SystemBrowser.Firefox
{
    public class FirefoxSystemBrowser : SystemBrowserBase, IFirefoxSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

        public FirefoxSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
             IUserDefaultSettingsService userDefaultsSettingsService
            )
        {
            _eventAggregator = eventAggregator;
            _applicationEnvironment = applicationEnvironment;
            _systemBrowserInfoManager = systemBrowserInfoManager;
            _userDefaultsSettingsService = userDefaultsSettingsService;
        }

        public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
        {
            return new FirefoxSystemBrowserInstance(
                _eventAggregator,
                o,
                _userDefaultsSettingsService,
                _applicationEnvironment.ApplicationDataFolderPath,
                GetBrowserExePath());
        }

        private string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("firefox")
                .Path;
        }
    }
}
