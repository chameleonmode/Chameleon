using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Browsers.Brave
{
    public class BraveSystemBrowser : SystemBrowserBase, IBraveSystemBrowser
    {
        protected readonly IEventAggregator _eventAggregator;
        protected readonly IApplicationEnvironment _applicationEnvironment;
        protected readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        protected readonly ISetPreferencesService _setPreferencesService;
        protected readonly IUserDefaultSettingsService _userDefaultsSettingsService;

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

        protected string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("brave")
                .Path;
        }
    }
}
