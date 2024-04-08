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

namespace Chameleon.SystemBrowser.Chrome
{
    public class ChromeSystemBrowserInstance : SystemBrowserInstance
    {
        private readonly ISetPreferencesService _setPreferencesService;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ChameleonChromeExtension _chromeExtension;

        public ChromeSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            ISetPreferencesService setPreferencesService,
            IApplicationEnvironment applicationEnvironment,
            string browserExeFilePath
            ) : base(eventAggregator, options, applicationEnvironment.ApplicationDataFolderPath, browserExeFilePath)
        {
            _setPreferencesService = setPreferencesService;
            _applicationEnvironment = applicationEnvironment;
            _chromeExtension = new ChameleonChromeExtension(_applicationEnvironment);
        }

        protected override SystemBrowserType BrowserType => SystemBrowserType.Chrome;

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
                arguments.Append($"\"{urlToOpen.AbsoluteUri}\" ");
            }

            arguments.Append("--no-default-browser-check ");

            //var userDataFolder = _browserProfileFolderPath;
            arguments.Append($"--user-data-dir=\"{_browserProfileFolderPath}\" ");

            var extensionDirectories = GetLoadExtensionsArgument();
            //foreach (var ext in extensionDirectories.Split(','))
            //{
            //    arguments.Append($"--load-extension={ext} ");
            //}
            //arguments.Append(@$"--load-extension='C:\Users\elimd\AppData\Local\Google\Chrome\User Data\Default\Extensions\bkmmlbllpjdpgcgdohbaghfaecnddhni\0.2.2_0' ");
            //arguments.Append(@$"--disable-extensions-except='C:\Users\elimd\AppData\Local\Google\Chrome\User Data\Default\Extensions\bkmmlbllpjdpgcgdohbaghfaecnddhni\0.2.2_0' ");
            arguments.Append($"--load-extension={extensionDirectories}");

            var webBrowser = UserProfile.WebBrowser;
            if (!webBrowser.WebRTC)
            {
                arguments.Append("--disable-media-stream ");
                arguments.Append("--disable-webrtc-hw-encoding ");
                arguments.Append("--disable-webrtc-hw-decoding ");
                arguments.Append("--webrtc-stun-probe-trial ");
                arguments.Append("--use-fake-device-for-media-stream ");
                arguments.Append("--enable-webrtc-hide-local-ips-with-mdns ");
                arguments.Append("--force-webrtc-ip-handling-policy ");
                arguments.Append("--enforce-webrtc-ip-permission-check ");
            }

            if (!webBrowser.WebGL)
            {
                arguments.Append("--disable-webgl ");
            }

            if (!webBrowser.Tracking)
            {
                // not disable tracking totally, but disable for hyperlink
                arguments.Append("--disable-hyperlink-auditing ");
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
            //arguments.Append($"--remote-debugging-port={NextFreePort(1000)} ");
            return arguments.ToString();
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
