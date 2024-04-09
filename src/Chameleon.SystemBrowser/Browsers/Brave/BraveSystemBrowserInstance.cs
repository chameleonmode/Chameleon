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
        private readonly ChameleonBraveExtension _chromeExtension;

        public BraveSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            ISetPreferencesService setPreferencesService,
            IApplicationEnvironment applicationEnvironment,
            string browserExeFilePath
            ) : base(eventAggregator, options, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
        {
            _setPreferencesService = setPreferencesService;
             _chromeExtension = new ChameleonBraveExtension(applicationEnvironment);
        }

        protected override SystemBrowserType BrowserType => SystemBrowserType.Brave;

        protected override string GetCommandLineArguments()
        {
            var arguments = new StringBuilder(1024);
            arguments.Append("--new-window ");
            var urlToOpen = Options.Url;
            //if (Options.SignIn)
            //{
            //    urlToOpen = _chromeExtension.GetUrlToOpen(urlToOpen);
            //}

            if (urlToOpen != null)
            {
                arguments.Append($"\"{urlToOpen}\" ");
            }

            //arguments.Append($"--user-data-dir=\"{_browserProfileFolderPath}\" ");

            var extensionDirectories = GetLoadExtensionsArgument();
            arguments.Append($"--load-extension={extensionDirectories} ");
            //arguments.Append($"--disable-extensions-except={extensionDirectories} ");

            var webBrowser = UserProfile.WebBrowser;
            if (!webBrowser.WebRTC)
            {
                arguments.Append("--disable-webrtc-hw-encoding ");
                arguments.Append("--disable-webrtc-hw-decoding ");
            }

            if (!webBrowser.WebGL)
            {
                arguments.Append("--disable-webgl ");
                arguments.Append("--disable-3d-apis ");
            }


            //var proxy = UserProfile.Proxy;
            //if (proxy.CanUse)
            //{
            //    //_dynamicProxyServer = DynamicProxyServerFactory.Create(proxy);
            //    //if (!_dynamicProxyServer.IsCertificateTrusted())
            //    //{
            //    //    // In case when windows popup/messagebox for trust proxy certificate was aborted by user by mistake
            //    //    // we anyway will allow to use browser and proxy by ignoring certificates
            //    //    arguments.Append("--ignore-certificate-errors ");
            //    //}

            //    //arguments.Append($"--proxy-server={_dynamicProxyServer.Server} ");
            //    arguments.Append($"--proxy-server={UserProfile.Proxy.Host}:{UserProfile.Proxy.Port} ");
            //}
            arguments.Append($"--remote-debugging-port={NextFreePort(1000)} ");
            return arguments.ToString();
        }

        protected override void InitializeExtensionPath()
        {
            base.InitializeExtensionPath();
            EnsureExtensionsFolderExistsAsCopyFrom(SystemBrowserType.Chrome);
        }

        public override string GetLoadExtensionsArgument()
        {
            return _chromeExtension.GetLoadExtensionsArgument(
                _browserExtensionsFolderPath, UserProfile
                );
        }

        protected override void InitializeProfileFolder()
        {
            _setPreferencesService.SetPreferences(UserProfile.WebBrowser, _browserProfileFolderPath, BrowserType);
        }
    }
}