using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowser : IChromeSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        private readonly ISetPreferencesService _setPreferencesService;

        public ChromeSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager,
            ISetPreferencesService setPreferencesService
            )
        {
            _eventAggregator = eventAggregator;
            _applicationEnvironment = applicationEnvironment;
            _systemBrowserInfoManager = systemBrowserInfoManager;
            _setPreferencesService = setPreferencesService;
        }

        public void Open(ISystemBrowserLaunchOptions options)
        {
            var browser = new ChromeSystemBrowserInstance(
                _eventAggregator,
                options,
                _setPreferencesService,
                _applicationEnvironment,
                GetBrowserExePath()
                );
            browser.Open();
        }

        private string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("chrome")
                .Path;
        }
    }
}
