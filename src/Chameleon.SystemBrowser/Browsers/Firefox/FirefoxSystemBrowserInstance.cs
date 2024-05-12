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
using System.Text.RegularExpressions;

namespace Chameleon.SystemBrowser.Firefox
{
    //public class FirefoxWithExtension : Microsoft.Playwright.Core.BrowserType
    //{
    //    public FirefoxWithExtension(ChannelOwner parent, string guid, BrowserTypeInitializer initializer) : base(parent, guid, initializer)
    //    {
    //    }
    //}
    public partial class FirefoxSystemBrowserInstance : SystemBrowserInstance
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
            if (!Directory.Exists(_browserProfileFolderPath))
                Directory.CreateDirectory(_browserProfileFolderPath);

           
            var prefs = await InitializePrefsJs();

            return;
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

            BrowserContext = await SystemBrowserManager.Blaywright.Firefox.LaunchPersistentContextAsync(
                _browserProfileFolderPath,
                new()
                {
                    ExecutablePath = Path.Combine(Directory.GetCurrentDirectory(), @"firefox-1447\firefox\firefox.exe"), //@"C:\Program Files\Mozilla Firefox\firefox.exe",//Path.Combine(Directory.GetCurrentDirectory(), @"firefox-1447\firefox\firefox.exe"),// @"C:\dev\browsers\Firefox-124\firefox.exe",// Path.Combine(Directory.GetCurrentDirectory(), @"firefox-1447\firefox\firefox.exe"),
                    Args = new[]
                    {
                        "--allow-downgrade",
                        "--start-maximized",
                        $"--start-debugger-server {Port}"
                    },
                    //IgnoreDefaultArgs = new[] { "-silent" },
                    Env = new Dictionary<string, string>()
                    {
                        ["MOZ_REMOTE_SETTINGS_DEVTOOLS"] = "1"
                    },

                    Headless = false,
                    Proxy = proxy,
                    ViewportSize = ViewportSize.NoViewport,
                    FirefoxUserPrefs = prefs,
                });
        }

        // TODO: refactor next legacy code
        private async Task<Dictionary<string, object>> InitializePrefsJs()
        {
            var prefs = new Dictionary<string, object>()
            {

                // Only allow the old modal dialogs. This should be removed when there is
                // support for the new modal UI (see Bug 1686743).
                ["prompts.contentPromptSubDialog"] = false,
                ["alerts.useSystemBackend"] = false,
                ["app.normandy.first_run"] = false,
                //["prompts.modalType.confirmAuth"] = 2,
                //["privacy.authPromptSpoofingProtection"] = false,
                //["prompts.defaultModalType"] = 1,
                //["prompts.windowPromptSubDialog"] = false,
                //["browser.startup.windowsLaunchOnLogin.disableLaunchOnLoginPrompt"] = true,
                //["network.auth.subresource-http-auth-allow"] = 2,
                //["prompts.authentication_dialog_abuse_limit"] = -1,
                ["browser.newtab.preload"] = false,
                ["app.shield.optoutstudies.enabled"] = false,
                ["extensions.pendingOperations"] = false,
                ["media.hardware-video-decoding.failed"] = false,
                ["sanity-test.running"] = false,
                // =================================================================
                // THESE ARE THE PROPERTIES THAT MUST BE ENABLED FOR JUGGLER TO WORK
                // =================================================================
                ["dom.input_events.security.minNumTicks"] = 0,
                ["dom.input_events.security.minTimeElapsedInMS"] = 0,
                ["dom.iframe_lazy_loading.enabled"] = false,
                ["datareporting.policy.dataSubmissionEnabled"] = false,
                ["datareporting.policy.dataSubmissionPolicyAccepted"] = false,
                ["datareporting.policy.dataSubmissionPolicyBypassNotification"] = false,
                // Force pdfs into downloads.
                //pref("pdfjs.disabled", true);
                // This preference breaks our authentication flow.  
                ["network.auth.use_redirect_for_retries"] = false,
                // Disable cross-process iframes, but not cross-process navigations.  
                ["fission.webContentIsolationStrategy"] = 0,
                // Disable BFCache in parent process.
                // We also separately disable BFCache in content via docSchell property.  
                ["fission.bfcacheInParent"] = false,
                // Disable first-party-based cookie partitioning.
                // When it is enabled, we have to retain "thirdPartyCookie^" permissions
                // in the storageState.      
                ["network.cookie.cookieBehavior"] = 4,
                // Increase max number of child web processes so that new pages
                // get a new process by default and we have a process isolation
                // between pages from different contexts. If this becomes a performance
                // issue we can povide custom '@mozilla.org/ipc/processselector;1'    
                ["dom.ipc.processCount"] = 60000,

                // Never reuse processes as they may keep previously overridden values
                // (locale, timezone etc.).       
                ["dom.ipc.processPrelaunch.enabled"] = false,
                // Isolate permissions by user context.      
                ["permissions.isolateBy.userContex"] = true,

                // Allow creating files in content process - required for
                // |Page.setFileInputFiles| protocol method. 
                ["dom.file.createInChild"] = true,
                // Do not warn when closing all open tabs   
                ["browser.tabs.warnOnClose"] = false,
                // Do not warn when closing all other open tabs   
                ["browser.tabs.warnOnCloseOtherTabs"] = false,
                // Do not warn when multiple tabs will be opened    
                ["browser.tabs.warnOnOpen"] = false,
                // Do not warn on quitting Firefox     
                ["browser.warnOnQuit"] = false,
                // Disable popup-blocker
                //pref("dom.disable_open_during_load", false);
                // Disable the ProcessHangMonitor        
                ["dom.ipc.reportProcessHangs"] = false,
                ["hangmonitor.timeou"] = 0,
                // Allow the application to have focus even it runs in the background 
                ["focusmanager.testmode"] = true,
                // No ICC color correction. We need this for reproducible screenshots.
                // See https://developer.mozilla.org/en/docs/Mozilla/Firefox/Releases/3.5/ICC_color_correction_in_Firefox.
                //pref("gfx.color_management.mode", 0);
                //pref("gfx.color_management.rendering_intent", 3);
                // Always use network provider for geolocation tests so we bypass the
                // macOS dialog raised by the corelocation provider   
                ["geo.provider.testing"] = true,
                // =================================================================
                // THESE ARE NICHE PROPERTIES THAT ARE NICE TO HAVE
                // =================================================================
                // Enable software-backed webgl. See https://phabricator.services.mozilla.com/D164016
                //pref("webgl.forbid-software", false);
                // Disable auto-fill for credit cards and addresses.
                // See https://github.com/microsoft/playwright/issues/21393
                //pref("extensions.formautofill.creditCards.supported", "off");
                //pref("extensions.formautofill.addresses.supported", "off");
                // Allow access to system-added self-signed certificates. This aligns
                // firefox behavior with other browser defaults.
                ["security.enterprise_roots.enabled"] = true,
                // Avoid stalling on shutdown, after "xpcom-will-shutdown" phase.
                // This at least happens when shutting down soon after launching.
                // See AppShutdown.cpp for more details on shutdown phases.
                ["toolkit.shutdown.fastShutdownStage"] = 3,
                // Use light theme by default.
                //pref("ui.systemUsesDarkTheme", 0);
                // Do not use system colors - they are affected by themes.
                ["ui.use_standins_for_native_colors"] = true,
                // Turn off the Push service.
                ["dom.push.serverURL"] = "",
                // Prevent Remote Settings (firefox.settings.services.mozilla.com) to issue non local connections.
                ["services.settings.server"] = "",
                // Prevent location.services.mozilla.com to issue non local connections.
                ["browser.region.network.url"] = "",
                ["browser.pocket.enabled"] = false,
                ["browser.newtabpage.activity-stream.feeds.topsites"] = false,
                // required to prevent non-local access to push.services.mozilla.com
                ["dom.push.connection.enabled"] = false,
                // Prevent contile.services.mozilla.com to issue non local connections.
                ["browser.topsites.contile.enabled"] = false,
                ["browser.safebrowsing.provider.mozilla.updateURL"] = "",
                ["browser.library.activity-stream.enabled"] = false,
                ["browser.search.geoSpecificDefaults"] = false,
                ["browser.search.geoSpecificDefaults.url"] = "",
                ["captivedetect.canonicalURL"] = "",
                ["network.captive-portal-service.enabled"] = false,
                ["network.connectivity-service.enabled"] = false,
                ["browser.newtabpage.activity-stream.asrouter.providers.snippets"] = "",
                // Make sure Shield doesn't hit the network.
                ["app.normandy.api_url"] = "",
                ["app.normandy.enabled"] = false,
                // Disable updater
                ["app.update.enabled"] = false,
                // Disable Firefox old build background check   
                ["app.update.checkInstallTim"] = false,
                // Disable automatically upgrading Firefox     
                ["app.update.disabledForTesting"] = true,
                // make absolutely sure it is really off
                ["app.update.auto"] = false,
                ["app.update.mode"] = 0,
                ["app.update.service.enabled"] = false,
                // Dislabe newtabpage    
                ["browser.startup.homepage"] = "about:blank",
                ["browser.newtabpage.enabled"] = false,
                ["browser.startup.page"] = 3, // 0 for no restore  3 for restore
                // Do not redirect user when a milstone upgrade of Firefox is detected
                ["browser.startup.homepage_override.mstone"] = "ignore",
                // Disable topstories                       
                ["browser.newtabpage.activity-stream.feeds.section.topstories"] = false,
                // DevTools JSONViewer sometimes fails to load dependencies with its require.js.
                // This spams console with a lot of unpleasant errors.
                // (bug 1424372)
                ["devtools.jsonview.enabled"] = false,
                // Increase the APZ content response timeout in tests to 1 minute.
                // This is to accommodate the fact that test environments tends to be
                // slower than production environments (with the b2g emulator being
                // the slowest of them all), resulting in the production timeout value
                // sometimes being exceeded and causing false-positive test failures.
                //
                // (bug 1176798, bug 1177018, bug 1210465)
                ["apz.content_response_timeout"] = 60000,
                // Indicate that the download panel has been shown once so that
                // whichever download test runs first doesn't show the popup
                // inconsistently.
                ["browser.download.panel.shown"] = true,
                // Background thumbnails in particular cause grief, and disabling
                // thumbnails in general cannot hurt
                ["browser.pagethumbnails.capturing_disabled"] = true,
                // Disable safebrowsing components.    
                ["browser.safebrowsing.blockedURIs.enabled"] = false,
                ["browser.safebrowsing.downloads.enabled"] = false,
                ["browser.safebrowsing.passwords.enabled"] = false,
                ["browser.safebrowsing.malware.enabled"] = false,
                ["browser.safebrowsing.phishing.enabled"] = false,
                // Disable updates to search engines.
                ["browser.search.update"] = false,
                // Turn off search suggestions in the location bar so as not to trigger
                // network connections.
                ["browser.urlbar.suggest.searches"] = true,
                // Do not restore the last open set of tabs if the browser has crashed
                ["browser.sessionstore.resume_from_crash"] = false,
                // Don't check for the default web browser during startup.
                ["browser.shell.checkDefaultBrowser"] = false,
                // Disable browser animations (tabs, fullscreen, sliding alerts)
                ["toolkit.cosmeticAnimations.enabled"] = false,
                // Close the window when the last tab gets closed
                ["browser.tabs.closeWindowWithLastTab"] = true,
                // Do not allow background tabs to be zombified on Android, otherwise for
                // tests that open additional tabs, the test harness tab itself might get
                // unloaded
                //pref("browser.tabs.disableBackgroundZombification", false);
                // Disable first run splash page on Windows 10
                ["browser.usedOnWindows10.introURL"] = "",
                // Disable the UI tour.
                //
                // Should be set in profile.
                ["browser.uitour.enabled"] = false,
                // Do not show datareporting policy notifications which can
                // interfere with tests    
                ["datareporting.healthreport.documentServerURI"] = "",
                ["datareporting.healthreport.about.reportUrl"] = "",
                ["datareporting.healthreport.logging.consoleEnabled"] = false,
                ["datareporting.healthreport.service.enabled"] = false,
                ["datareporting.healthreport.service.firstRun"] = false,
                ["datareporting.healthreport.uploadEnabled"] = false,
                // Automatically unload beforeunload alerts  
                ["dom.disable_beforeunload"] = false,
                // Disable slow script dialogues    
                ["dom.max_chrome_script_run_time"] = 0,
                ["dom.max_script_run_time"] = 0,
                // Only load extensions from the application and user profile
                // AddonManager.SCOPE_PROFILE + AddonManager.SCOPE_APPLICATION
                //pref("extensions.autoDisableScopes", 0);
                //pref("extensions.enabledScopes", 15);
                // Disable metadata caching for installed add-ons by default
                //pref("extensions.getAddons.cache.enabled", false);
                // Disable installing any distribution extensions or add-ons.
                // pref("extensions.installDistroAddons", false);
                // Turn off extension updates so they do not bother tests
                //pref("extensions.update.enabled", false);
                // pref("extensions.update.notifyUser", false);
                // Make sure opening about:addons will not hit the network   
                ["extensions.webservice.discoverURL"] = "",
                //pref("extensions.screenshots.disabled", true);
                //pref("extensions.screenshots.upload-disabled", true);
                // Disable useragent updates
                //pref("general.useragent.updates.enabled", false);   
                // Do not scan Wifi    
                ["geo.wifi.scan"] = false,
                // Show chrome errors and warnings in the error console
                ["javascript.options.showInConsole"] = true,
                // Disable download and usage of OpenH264: and Widevine plugins
                // pref("media.gmp-manager.updateEnabled", false);
                // Do not prompt with long usernames or passwords in URLs 
                ["network.http.phishy-userpass-length"] = 255,
                // Do not prompt for temporary redirects         
                ["network.http.prompt-temp-redirect"] = false,
                // Disable speculative connections so they are not reported as leaking
                // when they are hanging around
                ["network.http.speculative-parallel-limit"] = 0,
                // Do not automatically switch between offline and online  
                ["network.manage-offline-status"] = false,
                // Make sure SNTP requests do not hit the network
                ["network.sntp.pools"] = "",
                ["security.certerrors.mitm.priming.enabled"] = false,
                // Local documents have access to all other local documents,
                // including directory listings
                ["security.fileuri.strict_origin_policy"] = false,
                // Tests do not wait for the notification button security delay  
                ["security.fileuri.notification_enable_delay"] = 0,
                // Do not automatically fill sign-in forms with known usernames and
                // passwords
                //pref("signon.autofillForms", false);
                // Disable password capture, so that tests that include forms are not
                // influenced by the presence of the persistent doorhanger notification
                //pref("signon.rememberSignons", false);
                // Disable first-run welcome page  
                ["startup.homepage_welcome_url"] = "about:blank",
                ["startup.homepage_welcome_url.additional"] = "",
                // Prevent starting into safe mode after application crashes  
                ["toolkit.startup.max_resumed_crashes"] = -1,
                ["toolkit.crashreporter.enabled"] = false,
                ["toolkit.telemetry.enabled"] = false,
                ["toolkit.telemetry.server"] = "",
                // Disable downloading the list of blocked extensions. 
                ["extensions.blocklist.enabled"] = false,
                // Force Firefox Devtools to open in a separate window.
                //pref("devtools.toolbox.host", "window");
                // Disable auto translations
                //pref("browser.translations.enable", false);
                // Disable spell check
                //pref("layout.spellcheckDefault", 0);
                ["webgl.disabled"] = !UserProfile.WebBrowser.WebGL,
                ["media.navigator.enabled"] = UserProfile.WebBrowser.WebRTC,
                ["media.peerconnection.enabled"] = UserProfile.WebBrowser.WebRTC,
                ["plugin.state.flash"] = UserProfile.WebBrowser.Flash ? 1 : 0,
                ["plugins.flashBlock.enabled"] = !UserProfile.WebBrowser.Flash,
                ["privacy.donottrackheader.enabled"] = !UserProfile.WebBrowser.Tracking,
                ["privacy.trackingprotection.enabled"] = UserProfile.WebBrowser.Tracking,
                ["plugin.state.npctr"] = 0,
                ["plugin.state.java"] = 0,
                ["devtools.debugger.remote-enabled"] = true,
                ["devtools.debugger.prompt-connection"] = false,
                ["extensions.manifestV3.enabled"] = true,
                ["xpinstall.signatures.required"] = false,
                ["dom.forms.autocomplete.formautofill"] = true,
                ["signon.autologin.proxy"] = true,
                ["network.auth.use-sspi"] = false,

                ["network.proxy.type"] = 1,
            };
            if (UserProfile.Proxy.CanUse)
            {
                var host = UserProfile.Proxy.Host; 
                var port = UserProfile.Proxy.Port;
                prefs["network.proxy.http"] = host;
                prefs["network.proxy.http_port"] = port;
                prefs["network.proxy.backup.http"] = host;
                prefs["network.proxy.backup.http_port"] = port;
                prefs["network.proxy.ssl"] = host;
                prefs["network.proxy.ssl_port"] = port;
                prefs["network.proxy.backup.ssl"] = host;
                prefs["network.proxy.backup.ssl_port"] = port;
            }

            // Define a regular expression pattern to extract key-value pairs
            Regex regex = UserPrefRegex();
                                                                           
            var prefsFilePath = Path.Combine(_browserProfileFolderPath, "prefs.js");
            if(File.Exists(prefsFilePath))
            {
               foreach(var userPref in await File.ReadAllLinesAsync(prefsFilePath))
                {
                    if (!userPref.HasAny()) continue;
                    // Match the pattern in the input string
                    Match match = regex.Match(userPref);

                    // If the pattern is found, extract key-value pairs
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value.Trim('"');

                        // Add key-value pair to the dictionary
                        if(!prefs.ContainsKey(key) && !key.Contains(".proxy."))
                            prefs[key] = value;
                    }

                }
            }
            List<string> filePrefs = [];
            foreach (var item in prefs)
            {
                filePrefs.Add($"user_pref(\"{item.Key}\", {ParseValue(item.Value.ToString())});");
            }
            await File.WriteAllLinesAsync(prefsFilePath, filePrefs);
            return prefs;
        }
        object ParseValue(string value)
        {
            // Try parsing value as int
            if (int.TryParse(value, out int intValue))
                return intValue;

            // Try parsing value as bool
            if (bool.TryParse(value, out bool boolValue))
                return boolValue.ToString().ToLower();

            // Otherwise, treat it as a string
            return $"\"{value}\"";
        }

        protected override string GetCommandLineArguments()
        {

            return string.Join(" ", [
                "-new-instance",
                "-wait-for-browser",
                $"-new-window {Starturl}",
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

        [GeneratedRegex(@"user_pref\(""(.*?)"", (\""(.*?)\""|.*?)\);")]
        private static partial Regex UserPrefRegex();
    }
}
