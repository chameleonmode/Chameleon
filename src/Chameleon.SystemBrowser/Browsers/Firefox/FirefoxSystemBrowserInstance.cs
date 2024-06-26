namespace Chameleon.SystemBrowser.Firefox;

public partial class FirefoxSystemBrowserInstance(
        IEventAggregator eventAggregator,
        ISystemBrowserLaunchOptions options,
        IUserDefaultSettingsService userDefaultsSettingsService,
        string browserDataFolderPath) 
    : SystemBrowserInstance(eventAggregator, options, userDefaultsSettingsService, browserDataFolderPath)
{
    protected override SystemBrowserType BrowserType => SystemBrowserType.Firefox;

    protected override async Task InitializeProfileFolder()
    {
         _ = await InitializePrefsJs();
    }

    // TODO:
    private async Task<Dictionary<string, object>> InitializePrefsJs()
    {
        var prefs = new Dictionary<string, object>()
        {

            // =================================================================
            // THESE ARE THE PROPERTIES FROM https://arkenfox.github.io/gui/
            // =================================================================  
            //[SECTION 0100]: STARTUP        
            // Dislabe newtabpage    
            ["browser.startup.homepage"] = "about:blank",
            ["browser.newtabpage.enabled"] = false,
            ["browser.startup.page"] = 3, //0=blank, 1=home, 2=last visited page, 3=resume previous session
            ["browser.newtabpage.activity-stream.showSponsored"] = false,
            ["browser.newtabpage.activity-stream.showSponsoredTopSites"] = false,
            ["browser.newtabpage.activity-stream.default.sites"] = "",
            //[SECTION 0200]: GEOLOCATION 
            ["geo.provider.network.url"] = "https://location.services.mozilla.com/v1/geolocate?key=%MOZILLA_API_KEY%",
            ["geo.provider.network.logging.enabled"] = true,
            ["geo.provider.ms-windows-location"] = false,
            ["geo.provider.use_corelocation"] = false,
            ["geo.provider.use_gpsd"] = false,
            ["geo.provider.use_geoclue"] = false,
            //[SECTION 0300]: QUIETER FOX
            //  RECOMMENDATIONS
            ["extensions.getAddons.showPane"] = false,
            ["extensions.htmlaboutaddons.recommendations.enabled"] = false,
            ["browser.discovery.enabled"] = false,
            ["browser.shopping.experience2023.enabled"] = false,
            //  TELEMETRY
            ["datareporting.policy.dataSubmissionEnabled"] = false,
            ["datareporting.healthreport.uploadEnabled"] = false,
            ["toolkit.telemetry.unified"] = false,
            ["toolkit.telemetry.enabled"] = false,
            ["toolkit.telemetry.server"] = "data:,",
            ["toolkit.telemetry.archive.enabled"] = false,
            ["toolkit.telemetry.newProfilePing.enabled"] = false,
            ["toolkit.telemetry.shutdownPingSender.enabled"] = false,
            ["toolkit.telemetry.updatePing.enabled"] = false,
            ["toolkit.telemetry.bhrPing.enabled"] = false,
            ["toolkit.telemetry.firstShutdownPing.enabled"] = false,
            ["toolkit.telemetry.coverage.opt-out"] = true,
            ["toolkit.coverage.opt-out"] = true,
            ["toolkit.coverage.endpoint.base"] = "",
            ["browser.newtabpage.activity-stream.feeds.telemetry"] = false,
            ["browser.newtabpage.activity-stream.telemetry"] = false,
            //  STUDIES
            ["app.shield.optoutstudies.enabled"] = false,
            ["app.normandy.enabled"] = false,
            ["app.normandy.api_url"] = "",
            //  CRASH REPORTS
            ["breakpad.reportURL"] = "",
            ["browser.tabs.crashReporting.sendReport"] = false,
            ["browser.crashReports.unsubmittedCheck.enabled"] = false,
            ["browser.crashReports.unsubmittedCheck.autoSubmit2"] = false,
            //	OTHER
            ["captivedetect.canonicalURL"] = "",
            //["network.captive-portal-service.enabled"] = false,
            //["network.connectivity-service.enabled"] = false, 
            //[SECTION 0400]: SAFE BROWSING (SB)   
            ["browser.safebrowsing.malware.enabled"] = false,
            ["browser.safebrowsing.phishing.enabled"] = false,
            ["browser.safebrowsing.downloads.enabled"] = false,
            ["browser.safebrowsing.downloads.remote.enabled"] = false,
            ["browser.safebrowsing.downloads.remote.url"] = "",
            ["browser.safebrowsing.downloads.remote.block_potentially_unwanted"] = false,
            ["browser.safebrowsing.downloads.remote.block_uncommon"] = false,
            ["browser.safebrowsing.allowOverride"] = false,
            //[SECTION 0600]: BLOCK IMPLICIT OUTBOUND [not explicitly asked for - e.g. clicked on]
            ["network.prefetch-next"] = false,
            ["network.dns.disablePrefetch"] = true,
            ["network.dns.disablePrefetchFromHTTPS"] = true,
            ["network.predictor.enabled"] = false,
            ["network.predictor.enable-prefetch"] = false,
            //["network.http.speculative-parallel-limit"] = 0,
            ["browser.places.speculativeConnect.enabled"] = false,
            ["browser.send_pings"] = false,
            //[SECTION 0700]: DNS / DoH / PROXY / SOCKS 
            ["network.proxy.socks_remote_dns"] = true,
            ["network.file.disable_unc_paths"] = true,
            ["network.gio.supported-protocols"] = "",
            ["network.proxy.failover_direct"] = false,
            ["network.proxy.allow_bypass"] = false,
            /*** [SECTION 0700]: HTTP* / TCP/IP / DNS / PROXY / SOCKS etc https://github.com/arkenfox/user.js/blob/5bd5f6b28e801b8437e2574fad35f52365a6b593/user.js ***/
            ["network.dns.disableIPv6"] = true,
            ["network.ftp.enabled"] = false,
            ["browser.fixup.alternate.enabled"] = false,
            ["browser.casting.enabled"] = false,
            //["network.trr.mode"] = 3,
            //["network.trr.uri"] = $"{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}",
            //["network.trr.custom_uri"] = $"https://{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}",     
            //["network.trr.credentials"] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserProfile.Proxy.UserName}:{UserProfile.Proxy.Password}"))}",     
            //[SECTION 2000]: PLUGINS / MEDIA / WEBRTC    
            ["media.peerconnection.ice.proxy_only_if_behind_proxy"] = true,
            ["media.peerconnection.ice.default_address_only"] = true,
            ["media.peerconnection.ice.no_host"] = true,
            ["media.gmp-provider.enabled"] = false,
            //[SECTION 2600]: MISCELLANEOUS 
            ["permissions.manager.defaultsUrl"] = "",
            ["webchannel.allowObject.urlWhitelist"] = "",
            ["pdfjs.disabled"] = false,
            ["pdfjs.enableScripting"] = false,
            //[SECTION 2700]: ETP (ENHANCED TRACKING PROTECTION)  
            ["browser.contentblocking.category"] = "strict",
            ["privacy.antitracking.enableWebcompat"] = false,
            // =================================================================
            // THESE ARE THE PROPERTIES FROM https://mullvad.net/en/browser/hard-facts
            // =================================================================
            ["privacy.resistFingerprinting"] = true,
            ["privacy.resistFingerprinting.autoDeclineNoUserInputCanvasPrompts"] = true,
            ["privacy.resistFingerprinting.block_mozAddonManager"] = true,
            ["privacy.resistFingerprinting.exemptedDomains"] = "*.example.invalid",
            ["privacy.resistFingerprinting.jsmloglevel"] = "Warn",
            ["privacy.resistFingerprinting.letterboxing"] = true,
            ["privacy.resistFingerprinting.randomDataOnCanvasExtract"] = true,
            ["privacy.resistFingerprinting.reduceTimerPrecision.jitter"] = true,
            ["privacy.resistFingerprinting.reduceTimerPrecision.microseconds"] = 1000,
            ["privacy.resistFingerprinting.target_video_res"] = 480,
            ["privacy.resistFingerprinting.testGranularityMask"] = 0,
            ["services.sync.prefs.sync.privacy.resistFingerprinting.reduceTimerPrecision.jitter"] = true,
            ["services.sync.prefs.sync.privacy.resistFingerprinting.reduceTimerPrecision.microseconds"] = true,
            // Only allow the old modal dialogs. This should be removed when there is
            // support for the new modal UI (see Bug 1686743).
            //["prompts.contentPromptSubDialog"] = true,
            //["alerts.useSystemBackend"] = false,
            //["prompts.modalType.confirmAuth"] = 2,
            //["privacy.authPromptSpoofingProtection"] = false,
            //["prompts.defaultModalType"] = 1,
            //["prompts.windowPromptSubDialog"] = false,
            //["browser.startup.windowsLaunchOnLogin.disableLaunchOnLoginPrompt"] = true,
            // Turn off the authentication dialog blocking 
            ["network.negotiate-auth.allow-proxies"] = true,
            ["network.auth.subresource-http-auth-allow"] = 1,
            ["prompts.authentication_dialog_abuse_limit"] = -1,
            ["browser.newtab.preload"] = false,
            ["extensions.pendingOperations"] = false,
            ["media.hardware-video-decoding.failed"] = false,
            ["sanity-test.running"] = false,
            // =================================================================
            // THESE ARE THE PROPERTIES THAT MUST BE ENABLED FOR JUGGLER TO WORK
            // =================================================================
            ["dom.input_events.security.minNumTicks"] = 0,
            ["dom.input_events.security.minTimeElapsedInMS"] = 0,
            ["dom.iframe_lazy_loading.enabled"] = false,
            //["datareporting.policy.dataSubmissionEnabled"] = false,
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
            //["captivedetect.canonicalURL"] = "",
            ["network.captive-portal-service.enabled"] = false,
            ["network.connectivity-service.enabled"] = false,
            ["browser.newtabpage.activity-stream.asrouter.providers.snippets"] = "",
            // Make sure Shield doesn't hit the network.
            //["app.normandy.api_url"] = "",
            //["app.normandy.enabled"] = false,     
            //["app.normandy.first_run"] = false,
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
            ["browser.safebrowsing.passwords.enabled"] = false,
            //["browser.safebrowsing.downloads.enabled"] = false,
            //["browser.safebrowsing.malware.enabled"] = false,
            //["browser.safebrowsing.phishing.enabled"] = false,
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
            ["browser.uitour.url"] = "",
            // Do not show datareporting policy notifications which can
            // interfere with tests    
            ["datareporting.healthreport.documentServerURI"] = "",
            ["datareporting.healthreport.about.reportUrl"] = "",
            ["datareporting.healthreport.logging.consoleEnabled"] = false,
            ["datareporting.healthreport.service.enabled"] = false,
            ["datareporting.healthreport.service.firstRun"] = false,
            //["datareporting.healthreport.uploadEnabled"] = false,
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

        //commented out this still sometimes causes firefox system login popup window
        //if (UserProfile.Proxy.CanUse)
        //{
        //    var host = UserProfile.Proxy.Host;
        //    var port = UserProfile.Proxy.Port;
        //    prefs["network.proxy.http"] = host;
        //    prefs["network.proxy.http_port"] = port;
        //    prefs["network.proxy.backup.http"] = host;
        //    prefs["network.proxy.backup.http_port"] = port;
        //    prefs["network.proxy.ssl"] = host;
        //    prefs["network.proxy.ssl_port"] = port;
        //    prefs["network.proxy.backup.ssl"] = host;
        //    prefs["network.proxy.backup.ssl_port"] = port;
        //}

        // Define a regular expression pattern to extract key-value pairs
        Regex regex = UserPrefRegex();

        var prefsFilePath = Path.Combine(BrowserProfileFolderPath, "prefs.js");
        if (File.Exists(prefsFilePath))
        {
            foreach (var userPref in await File.ReadAllLinesAsync(prefsFilePath))
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
                    if (!prefs.ContainsKey(key) && !key.Contains(".proxy."))
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

    protected override List<string> GetCommandLineArgumentsList()
    {
        var arguments = new List<string>()
            {
                "-new-instance",
                "-wait-for-browser",
                //$"-new-window",
                $"-profile \"{BrowserProfileFolderPath}\"",
                //"-no-remote"
            };

        string startUrl = HasProxyLogin && Starturl.Contains(ProxyAddonUtil.DomainLevelDelimiter)
            ? "about:blank" //added for now to work around proxy refresh issue
            : Starturl;

        arguments.Add($"-url {startUrl}");
        return arguments;
    }

    protected override string GetCommandLineArguments()
    {
        List<string> argumentsList = GetCommandLineArgumentsList();
        return string.Join(" ", argumentsList);
    }

    public override string GetLoadExtensionsArgument() => string.Empty;

    [GeneratedRegex(@"user_pref\(""(.*?)"", (\""(.*?)\""|.*?)\);")]
    private static partial Regex UserPrefRegex();

    protected override async Task<string> InitializeExtensionPath()
    {
        string proxyextdir = Path.Combine(BrowserProfileFolderPath, ProxyAddonUtil.AutoProxyFolderName);
        string pxoyextFile = Path.Combine(proxyextdir, ProxyAddonUtil.FirefoxAutoProxyAddonName);

        await IOtil.DeleteFExists(pxoyextFile);
        await IOtil.DeleteDExistsAsync(proxyextdir);
        Directory.CreateDirectory(proxyextdir);

        if (HasProxyLogin)
        {
            string startUrl =
                Starturl.Contains(ProxyAddonUtil.UrlSchemeEnd)
                ? Starturl : $"{ProxyAddonUtil.HTTPSScheme}{Starturl}";

            string loadUrl =
                startUrl.Contains(ProxyAddonUtil.DomainLevelDelimiter)
                ? $", () => {{ browser.tabs.update({{ url:\"{startUrl}\" }}); }});" : ");";

            using (var fileStream = new FileStream(pxoyextFile, FileMode.CreateNew))
            {
                using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true);
                await AddFileToArchive("manifest.json", ProxyAddonUtil.GetManifest(), archive);
                await AddFileToArchive("background.js", ProxyAddonUtil.GetBgJs(loadUrl, UserProfile.Proxy), archive); ;
            }

            var mf = Path.Combine(pxoyextFile, "manifest.json");
            var bf = Path.Combine(pxoyextFile, "background.js");
            await IOtil.DeleteFExists(mf);
            await IOtil.DeleteFExists(bf);
        }

        return proxyextdir;
    }

    private static async Task AddFileToArchive(string fileName, string fileText, ZipArchive archive)
    {
        var zipArchiveManifest = archive.CreateEntry(fileName, CompressionLevel.Fastest);
        using var zipStream = zipArchiveManifest.Open();
        using var writer = new StreamWriter(zipStream);
        await writer.WriteAsync(fileText);
    }
}

