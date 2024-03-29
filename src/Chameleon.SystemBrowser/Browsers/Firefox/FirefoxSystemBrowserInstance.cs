using Chameleon.Core.Extensions;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Proxy;
using Chameleon.SystemBrowser.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chameleon.Prism.Events;

namespace Chameleon.SystemBrowser.Firefox
{
    public class FirefoxSystemBrowserInstance : SystemBrowserInstance
    {
        public FirefoxSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            string browserDataFolderPath,
            string browserExeFilePath
            ) : base(eventAggregator, options, browserDataFolderPath, browserExeFilePath)
        {
        }

        protected override SystemBrowserType BrowserType => SystemBrowserType.Firefox;
        protected override void OnProfileFolderCreated()
        {
            CreateProfile();
        }

        private void CreateProfile()
        {
            var createProfileProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = $"firefox -CreateProfile \"{UserProfile.Id} {_browserProfileFolderPath}\"",
                    FileName = _browserExeFilePath,
                }
            };

            createProfileProcess.Start();
            createProfileProcess.WaitForExit();
        }

        protected override void InitializeProfileFolder()
        {
            InitializePrefsJs();
        }

        // TODO: refactor next legacy code
        private void InitializePrefsJs()
        {
            var prefsFilePath = Path.Combine(_browserProfileFolderPath, "prefs.js");

            var fileTextLines = GetPerfsFileLines(prefsFilePath);

            //https://github.com/arkenfox/user.js/blob/master/user.js - all settings

            fileTextLines.Add("user_pref(\"plugin.state.npctrl\", 0);");
            fileTextLines.Add("user_pref(\"plugin.state.java\", 0);");
            fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart.2\", false);");
            fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart\", false);");
            fileTextLines.Add("user_pref(\"browser.shell.checkDefaultBrowser\", false);");
            fileTextLines.Add("user_pref(\"browser.tabs.closeWindowWithLastTab\", false);");
            fileTextLines.Add("user_pref(\"plugin.state.flash\", 2);");
            fileTextLines.Add("user_pref(\"plugins.flashBlock.enabled\", false);");
            fileTextLines.Add("user_pref(\"privacy.resistFingerprinting\", true);");
            fileTextLines.Add("user_pref(\"xpinstall.signatures.required\", false);");
            fileTextLines.Add("user_pref(\"xpinstall.whitelist.required\", false);");

            fileTextLines.Add($"user_pref(\"browser.startup.homepage\", \"{Options.Url}\");");

            var webBrowser = UserProfile.WebBrowser;
            if (!webBrowser.WebGL)
            {
                fileTextLines.Add("user_pref(\"webgl.disabled\", true);");
            }
            if (!webBrowser.WebRTC)
            {
                fileTextLines.Add("user_pref(\"media.navigator.enabled\", false);");
                fileTextLines.Add("user_pref(\"media.peerconnection.enabled\", false);");
            }
            if (!webBrowser.Flash)
            {
                fileTextLines.Add("user_pref(\"plugin.state.flash\", 0);");
                fileTextLines.Add("user_pref(\"plugins.flashBlock.enabled\", true);");
            }
            if (!webBrowser.Tracking)
            {
                fileTextLines.Add("user_pref(\"privacy.donottrackheader.enabled\", true);");
                fileTextLines.Add("user_pref(\"services.sync.prefs.sync.privacy.donottrackheader.enabled\", true);");
            }

            var proxy = UserProfile.Proxy;
            if (proxy.CanUse)
            {
                //_dynamicProxyServer = DynamicProxyServerFactory.Create(proxy);

                var host = UserProfile.Proxy.Host; //_dynamicProxyServer.Host;
                var port = UserProfile.Proxy.Port; //_dynamicProxyServer.Port;

                fileTextLines.Add("user_pref(\"network.proxy.type\", 1); ");
                fileTextLines.Add("user_pref(\"network.proxy.share_proxy_settings\", true);");

                fileTextLines.Add("user_pref(\"network.proxy.http\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.http_port\", " + port + ");");

                fileTextLines.Add("user_pref(\"network.proxy.ssl\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.ssl_port\", " + port + ");");

                fileTextLines.Add("user_pref(\"network.proxy.backup.ssl\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.ssl_port\", " + port + ");");

                fileTextLines.Add("user_pref(\"network.proxy.ftp\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.ftp_port\", " + port + ");");

                fileTextLines.Add("user_pref(\"network.proxy.socks\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.socks_port\", " + port + ");");

                fileTextLines.Add("user_pref(\"network.proxy.backup.socks\", \"" + host + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.socks_port\", " + port + ");");

                //if (!_dynamicProxyServer.IsCertificateTrusted())
                //{
                //    //https://www.techwalla.com/articles/how-to-disable-invalid-ssl-in-firefox
                //    // In case when windows popup/messagebox for trust proxy certificate was aborted by user by mistake
                //    // we anyway will allow to use browser and proxy by ignoring certificates
                //    fileTextLines.Add("user_pref(\"browser.ssl_override_behavior\", 1);");
                //}

                //pref("app.update.staging.enabled", true);
                //pref("app.update.service.enabled", true);
                //pref["app.update.enabled"] = false;
                //pref["app.update.autoUpdateEnabled"] = false;
                //pref["app.update.auto"] = false;
                //pref["app.update.mode"] = 0;
                //pref["app.update.service.enabled"] = false;
                //pref("browser.startup.homepage",            "about:home");
                //signon.autologin.proxy
            }
            File.WriteAllLines(prefsFilePath, fileTextLines.ToArray());
        }

        // TODO: refactor next legacy code
        private List<string> GetPerfsFileLines(string prefsFilePath)
        {
            var lines = new List<string>();
            if (!File.Exists(prefsFilePath))
            {
                return lines;
            }

            lines = File
                .ReadAllLines(prefsFilePath)
                .ToList();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (line.Contains("user_pref(\"plugin.state.npctrl\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"webgl.disabled\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"plugin.state.flash\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"plugin.state.java\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"media.peerconnection.enabled\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"privacy.donottrackheader.enabled\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"services.sync.prefs.sync.privacy.donottrackheader.enabled\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"browser.shell.checkDefaultBrowser\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"browser.tabs.closeWindowWithLastTab\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.type\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.http\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.http_port\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ssl\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ssl_port\", "))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.ssl\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.ssl_port\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ftp\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ftp_port\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.socks\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.socks_port\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.socks\","))
                {
                    lines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.socks_port\","))
                {
                    lines.RemoveAt(i);
                }
            }

            return lines;
        }

        protected override string GetCommandLineArguments()
        {
            var arguments = new StringBuilder(1024);
            arguments.Append($"-new-instance -profile \"{_browserProfileFolderPath}\"");

            //TODO: investigate how to install ecxtension via config file (*.ini)
            //var extensionsToInstal = GetLoadExtensionsArgument();
            //arguments.Append($"-install-extension {extensionsToInstal}");

            return arguments.ToString();
        }

        private string GetLoadExtensionsArgument()
        {
            return Directory
                .GetFiles(_browserExtensionsFolderPath)
                .AddQuotesToEachElement()
                .ToCommaSeparatedString();
        }
    }
}
