using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;
using Chameleon.SystemBrowser.Common;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowserInstance : SystemBrowserInstance
    {
        private readonly ISetPreferencesService _setPreferencesService;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

        protected override SystemBrowserType BrowserType => SystemBrowserType.Chrome;

        public ChromeSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            ISetPreferencesService setPreferencesService,
            IApplicationEnvironment applicationEnvironment,
            IUserDefaultSettingsService userDefaultsSettingsService,
            string browserExeFilePath
            ) : base(eventAggregator, options, userDefaultsSettingsService, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
        {
            _setPreferencesService = setPreferencesService;
        }

        protected override async Task InitializeProfileFolder()
        {
            await Task.Run(() => _setPreferencesService.SetPreferences(UserProfile.WebBrowser, _browserProfileFolderPath, BrowserType));
        }
    }
}
