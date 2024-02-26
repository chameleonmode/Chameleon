using Chameleon.Interfaces.WebBrowser;
using Chameleon.Interfaces.Environments;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Browsers.Brave
{
    public class BraveSystemBrowser : IBraveSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;
        private readonly ISetPreferencesService _setPreferencesService;

        public BraveSystemBrowser(
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
            var browser = new BraveSystemBrowserInstance(
                _eventAggregator,
                options,
                _setPreferencesService,
                _applicationEnvironment,
                "brave"
                );
            browser.Open();
        }

        private string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("brave")
                .Path;
        }
    }
}
