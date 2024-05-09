using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Common;
using Chameleon.SystemBrowser.Proxy;
using Chameleon.Prism.Events;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;
using Chameleon.Interfaces.Settings;

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
