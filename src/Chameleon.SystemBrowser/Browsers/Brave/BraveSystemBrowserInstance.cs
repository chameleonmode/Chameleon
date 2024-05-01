using System;
using System.Text;
using System.Web;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Chrome;
using Chameleon.SystemBrowser.Common;
using Chameleon.SystemBrowser.Proxy;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Browsers.Brave
{
    public class BraveSystemBrowserInstance : SystemBrowserInstance
    {
        private readonly ISetPreferencesService _setPreferencesService;

        protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;

        public BraveSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            ISetPreferencesService setPreferencesService,
            IApplicationEnvironment applicationEnvironment,
            string browserExeFilePath
            ) : base(eventAggregator, options, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
        {
            _setPreferencesService = setPreferencesService;
        }

        protected override async Task InitializeProfileFolder()
        {
            await Task.Run(()=>_setPreferencesService.SetPreferences(UserProfile.WebBrowser, _browserProfileFolderPath, BrowserType));
        }
    }
}