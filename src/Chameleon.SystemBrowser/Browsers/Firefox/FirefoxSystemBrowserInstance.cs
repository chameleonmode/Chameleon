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
using Microsoft.Playwright;
using System.Collections;
using Microsoft.Playwright.Transport;
using Microsoft.Playwright.Transport.Protocol;
using Chameleon.Interfaces.Settings;

namespace Chameleon.SystemBrowser.Firefox
{
    //public class FirefoxWithExtension : Microsoft.Playwright.Core.BrowserType
    //{
    //    public FirefoxWithExtension(ChannelOwner parent, string guid, BrowserTypeInitializer initializer) : base(parent, guid, initializer)
    //    {
    //    }
    //}
    public class FirefoxSystemBrowserInstance : SystemBrowserInstance
    {
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;
        public FirefoxSystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            IUserDefaultSettingsService userDefaultsSettingsService,
            string browserDataFolderPath,
            string browserExeFilePath
            ) : base(eventAggregator, options, userDefaultsSettingsService, browserDataFolderPath, browserExeFilePath)
        {
        }

        protected override SystemBrowserType BrowserType => SystemBrowserType.Firefox;
        //protected override Task OnProfileFolderCreated()
        //{
        //   // CreateProfile();
        //    return Task.CompletedTask;
        //}

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

        protected override async Task InitializeProfileFolder()
        {
            //await Task.Run(() => InitializePrefsJs());

            //await Task.Run(() => InitializePrefsJs());
            if (SystemBrowserManager.Blaywright == null)
                SystemBrowserManager.Blaywright = await Playwright.CreateAsync();

            Microsoft.Playwright.Proxy? proxy = null;
            if (HasProxyLogin)
            {
                proxy = new Microsoft.Playwright.Proxy()
                {
                    Server = $"http://{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}",
                    Username = UserProfile.Proxy.UserName,
                    Password = UserProfile.Proxy.Password,
                };
            }
            var prefs = new Dictionary<string, object>
            {
                ["browser.newtabpage.enabled"] = true,
                ["browser.startup.page"] = 3,
                ["signon.autofillForms"] = true,
                ["signon.rememberSignons"] = true,
                ["browser.urlbar.suggest.searches"] = true,
                ["browser.search.update"] = true,
                ["plugin.state.npctr"] = 0,
                ["plugin.state.java"] = 0,
                ["devtools.debugger.remote-enabled"]= true,
                ["devtools.debugger.prompt-connection"] = false,
                ["extensions.manifestV3.enabled"] = true,
                ["xpinstall.signatures.required"]  = false,
                ["dom.forms.autocomplete.formautofill"]  = true,
                //["extensions.installDistroAddons"]  = true,
                //["extensions.activeThemeID"] = "default-theme@mozilla.org",
                //["extensions.webextensions.uuids"] = "{\"formautofill@mozilla.org\":\"c330de13-262b-4cd6-a1b0-d21a562bddf2\",\"pictureinpicture@mozilla.org\":\"e83f77e3-edc8-49a1-9380-c41b262b6827\",\"screenshots@mozilla.org\":\"c7378971-4642-4032-88af-d3859d0dc38c\",\"webcompat-reporter@mozilla.org\":\"0d708515-bf56-4230-845d-f8968fdd804c\",\"webcompat@mozilla.org\":\"913bae20-34ba-49ac-81f3-b877eef1e835\",\"default-theme@mozilla.org\":\"a8f49007-78b9-49e8-972e-f45f0ea3474a\",\"addons-search-detection@mozilla.com\":\"27eee07a-9853-4705-a711-38401c593408\",\"google@search.mozilla.org\":\"ba5c7604-a4b9-44ed-8b12-23c372247a11\",\"wikipedia@search.mozilla.org\":\"6504e91a-fd2b-4978-88b1-42f394365fec\",\"bing@search.mozilla.org\":\"228d075e-3d66-4dfb-a3cf-213faec1c2fb\",\"ddg@search.mozilla.org\":\"1066adbc-d81a-469b-afa9-a95de4a49a6e\"}"
            };
            // Turn off search suggestions in the location bar so as not to trigger
            // network connections.
            //pref("browser.urlbar.suggest.searches", false);
            //user_pref("extensions.webextensions.uuids", "{\"formautofill@mozilla.org\":\"10b01988-a0ad-4f29-b09d-adf0d2153cfc\",\"pictureinpicture@mozilla.org\":\"bd05d1f6-6fd0-4a15-91cc-8034d73abc6a\",\"screenshots@mozilla.org\":\"dfec2e2f-9716-49e6-8893-204236c90ffa\",\"webcompat-reporter@mozilla.org\":\"12469d55-18f6-4c8d-8c25-b05575e57f01\",\"webcompat@mozilla.org\":\"8097d201-1756-4285-9f85-33c990f2bb72\",\"default-theme@mozilla.org\":\"25737c7c-4cd5-4860-8d62-149b04f02dec\",\"addons-search-detection@mozilla.com\":\"73db796e-3569-4fe8-8f12-55ced9147f48\",\"google@search.mozilla.org\":\"abd57346-3bf3-4681-8030-11602dd93cca\",\"wikipedia@search.mozilla.org\":\"ce750d58-35d5-47ac-a3d0-315885fca341\",\"bing@search.mozilla.org\":\"b565dc02-e1d5-4212-b690-b07fd5375d1b\",\"ddg@search.mozilla.org\":\"2cdfdb52-ef25-49a8-9587-4630cd212427\"}");
            // user_pref("browser.urlbar.placeholderName", "Google");
            var webBrowser = UserProfile.WebBrowser;
            if (!webBrowser.WebGL)
            {
                prefs["webgl.disabled"] = true;
            }
            if (!webBrowser.WebRTC)
            {
                prefs["media.navigator.enabled"] = false;
                prefs["media.peerconnection.enabled"] = false;
            }
            if (!webBrowser.Flash)
            {
                prefs["plugin.state.flash"] = 0;
                prefs["plugins.flashBlock.enabled"] = true;
            }
            if (!webBrowser.Tracking)
            {
                prefs["privacy.donottrackheader.enabled"] = true;
                prefs["privacy.donottrackheader.enabled"] = true;
            }
            BrowserContext = await SystemBrowserManager.Blaywright.Firefox.LaunchPersistentContextAsync(
                _browserProfileFolderPath,
                new()
                {
                    ExecutablePath = Path.Combine(Directory.GetCurrentDirectory(), @"firefox-1447\firefox\firefox.exe"),// @"C:\dev\browsers\Firefox-124\firefox.exe",// Path.Combine(Directory.GetCurrentDirectory(), @"firefox-1447\firefox\firefox.exe"),
                    Args = new[] 
                    {
                        "--allow-downgrade",
                        "--start-maximized", 
                        $"--start-debugger-server {Port}" 
                    },
                    //IgnoreDefaultArgs = new[] { "-silent" },
                    Headless = false,
                    Proxy = proxy,
                    ViewportSize = ViewportSize.NoViewport,
                    FirefoxUserPrefs = prefs,
                });

            // Force Firefox Devtools to open in a separate window.
            //pref("devtools.toolbox.host", "window");

            // Disable auto translations
            //pref("browser.translations.enable", false);

            // Disable spell check
            //pref("layout.spellcheckDefault", 0);

            // Do not automatically fill sign-in forms with known usernames and
            // passwords
            //pref("signon.autofillForms", false);

            // Disable password capture, so that tests that include forms are not
            // influenced by the presence of the persistent doorhanger notification
            //pref("signon.rememberSignons", false);

            // Disable installing any distribution extensions or add-ons.
            //pref("extensions.installDistroAddons", false);

            // Disable metadata caching for installed add-ons by default
            //pref("extensions.getAddons.cache.enabled", false);

            // Turn off search suggestions in the location bar so as not to trigger
            // network connections.
            //pref("browser.urlbar.suggest.searches", false);

            // Disable safebrowsing components.
            //pref("browser.safebrowsing.blockedURIs.enabled", false);
            //pref("browser.safebrowsing.downloads.enabled", false);
            //pref("browser.safebrowsing.passwords.enabled", false);
            //pref("browser.safebrowsing.malware.enabled", false);
            //pref("browser.safebrowsing.phishing.enabled", false);

            // Dislabe newtabpage
            //pref("browser.startup.homepage", "about:blank");
            //pref("browser.startup.page", 0);
            //pref("browser.newtabpage.enabled", false);

            // Use light theme by default.
            //pref("ui.systemUsesDarkTheme", 0);

            // Disable auto-fill for credit cards and addresses.
            // See https://github.com/microsoft/playwright/issues/21393
            //pref("extensions.formautofill.creditCards.supported", "off");
            //pref("extensions.formautofill.addresses.supported", "off");

            // Only load extensions from the application and user profile
            // AddonManager.SCOPE_PROFILE + AddonManager.SCOPE_APPLICATION
            //pref("extensions.autoDisableScopes", 0);
            //pref("extensions.enabledScopes", 5);

            // Turn off extension updates so they do not bother tests
            //pref("extensions.update.enabled", false);

            //pref("extensions.screenshots.disabled", true);
            //pref("extensions.screenshots.upload-disabled", true);

            // Disable updates to search engines.
            //pref("browser.search.update", false);
        }

        // TODO: refactor next legacy code
        private void InitializePrefsJs()
        {
            var prefsFilePath = Path.Combine(_browserProfileFolderPath, "prefs.js");

            var fileTextLines = GetPerfsFileLines(prefsFilePath);

            //https://github.com/arkenfox/user.js/blob/master/user.js - all settings

            fileTextLines.Add("user_pref(\"browser.startup.page\", 3);");

            //fileTextLines.Add("user_pref(\"signon.autologin.proxy\", true);");
            //fileTextLines.Add("user_pref(\"network.proxy.share_proxy_settings\", false);"); 
            ////fileTextLines.Add("user_pref(\"network.auth.use-sspi\", false);");
            //fileTextLines.Add("user_pref(\"network.negotiate-auth.allow-proxies\", true);");
            //fileTextLines.Add("user_pref(\"network.automatic-ntlm-auth.allow-proxies\", true);");
            //fileTextLines.Add("user_pref(\"network.automatic-ntlm-auth.allow-non-fqdn\", true);");
            //fileTextLines.Add("user_pref(\"network.negotiate-auth.allow-non-fqdn\", true);");
            ////pref("network.negotiate-auth.trusted-uris", site - list);
            ////pref("network.negotiate-auth.delegation-uris", site - list);
            ////pref("network.automatic-ntlm-auth.trusted-uris", site - list);
            //fileTextLines.Add($"user_pref(\"network.automatic-ntlm-auth.trusted-uris\",{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port},http://localhost);");
            //fileTextLines.Add($"user_pref(\"network.negotiate-auth.delegation-uris\", {UserProfile.Proxy.Host}:{UserProfile.Proxy.Port},http://localhost);");
            //fileTextLines.Add($"user_pref(\"network.negotiate-auth.trusted-uris\", {UserProfile.Proxy.Host}:{UserProfile.Proxy.Port},http://localhost);");

            //fileTextLines.Add("user_pref(\"extensions.getAddons.showPane\", false);");
            //fileTextLines.Add("user_pref(\"browser.discovery.enabled\", false);");
            //fileTextLines.Add("user_pref(\"browser.newtabpage.activity-stream.asrouter.userprefs.cfr.addons\", false);");
            //fileTextLines.Add("user_pref(\"browser.newtabpage.activity-stream.asrouter.userprefs.cfr.features\", false);");
            //fileTextLines.Add("user_pref(\"trailhead.firstrun.branches\", \"nofirstrun-empty\");");
            //fileTextLines.Add("user_pref(\"browser.aboutwelcome.enabled\", false);");

            //fileTextLines.Add("user_pref(\"plugin.state.npctrl\", 0);");
            //fileTextLines.Add("user_pref(\"plugin.state.java\", 0);");
            //fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart.2\", false);");
            //fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart\", false);");
            //fileTextLines.Add("user_pref(\"browser.shell.checkDefaultBrowser\", false);");
            //fileTextLines.Add("user_pref(\"browser.tabs.closeWindowWithLastTab\", false);");
            //fileTextLines.Add("user_pref(\"plugin.state.flash\", 2);");
            //fileTextLines.Add("user_pref(\"plugins.flashBlock.enabled\", false);");
            //fileTextLines.Add("user_pref(\"privacy.resistFingerprinting\", true);");
            //fileTextLines.Add("user_pref(\"xpinstall.signatures.required\", false);");
            //fileTextLines.Add("user_pref(\"xpinstall.whitelist.required\", false);");

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
                fileTextLines.Add($"user_pref(\"network.proxy.username\", {UserProfile.Proxy.UserName}); ");
                fileTextLines.Add($"user_pref(\"network.proxy.password \", {UserProfile.Proxy.Password}); ");
                //fileTextLines.Add("user_pref(\"network.proxy.share_proxy_settings\", true);");

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

            return string.Join(" ", [
                "-new-instance",
                "-wait-for-browser",
                $"-new-window about:blank",
                //$"-url \"{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}\"",
                $"-profile \"{_browserProfileFolderPath}\"",
                //"-no-remote"
                ]); 
            //TODO: investigate how to install ecxtension via config file (*.ini)
            //var extensionsToInstal = GetLoadExtensionsArgument();
            //arguments.Append($"-install-extension {extensionsToInstal}");
        }

        private string GetLoadExtensionsArgument()
        {
            return Directory
                .GetFiles(BrowserExtensionsFolderPath)
                .AddQuotesToEachElement()
                .ToCommaSeparatedString();
        }
    }
}
