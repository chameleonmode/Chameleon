using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Firefox
{
    public class FirefoxSystemBrowser : IFirefoxSystemBrowser
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ISystemBrowserInfoManager _systemBrowserInfoManager;

        public FirefoxSystemBrowser(
            IEventAggregator eventAggregator,
            IApplicationEnvironment applicationEnvironment,
            ISystemBrowserInfoManager systemBrowserInfoManager
            )
        {
            _eventAggregator = eventAggregator;
            _applicationEnvironment = applicationEnvironment;
            _systemBrowserInfoManager = systemBrowserInfoManager;
        }

        public void Open(ISystemBrowserLaunchOptions options)
        {
            var browser = new FirefoxSystemBrowserInstance(
                _eventAggregator,
                options,
                _applicationEnvironment.ApplicationDataFolderPath,
                "firefox.exe"
                );
            browser.Open();
        }

        private string GetBrowserExePath()
        {
            return _systemBrowserInfoManager
                .FindByName("firefox")
                .Path;
        }
    }
}
