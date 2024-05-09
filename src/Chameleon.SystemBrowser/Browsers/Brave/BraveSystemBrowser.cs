using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.Environments;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Chrome;
using Chameleon.Interfaces.Settings;

namespace Chameleon.SystemBrowser.Browsers.Brave
{
    public class BraveSystemBrowser : SystemBrowserBase, IBraveSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        private readonly ISetPreferencesService _setPreferencesService;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

        public BraveSystemBrowser(
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
            return new BraveSystemBrowserInstance(
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
                .FindByName("brave")
                .Path;
        }
    }
}
