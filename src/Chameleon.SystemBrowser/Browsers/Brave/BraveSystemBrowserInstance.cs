using System;
using System.Text;
using System.Web;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Chrome;
using Chameleon.SystemBrowser.Common;
using Chameleon.SystemBrowser.Proxy;
using Chameleon.Prism.Events;
using Chameleon.Interfaces.Settings;

namespace Chameleon.SystemBrowser.Browsers.Brave
{
    public class BraveSystemBrowserInstance : SystemBrowserInstance
    {
        private readonly ISetPreferencesService _setPreferencesService;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;

        protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;

        public BraveSystemBrowserInstance(
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
            await Task.Run(()=>_setPreferencesService.SetPreferences(UserProfile.WebBrowser, _browserProfileFolderPath, BrowserType));
        }
    }
}