//using Chameleon.Interfaces.UserProfiles;
//using Chameleon.SystemBrowser.Common;
//using Microsoft.Playwright;
//using System.Diagnostics;
//using System.Linq;

//namespace Chameleon.SystemBrowser.Automation;

//public class Play
//{
//    public static Play Instance { get; } = new Play();
//    readonly Dictionary<string, Process?> _processMap = [];
//    readonly Dictionary<string, IBrowser?> createdContext = [];
//    IPlaywright? playwright = null;
//    IBrowser? _browser = null;

//    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
//    private Play()
//    {
//        // Init();
//    }

//    static readonly IEnumerable<string> ignoreDefaultArgs = new[]
//    {
//        "about:blank",
//        "--enable-automation",
//        "--no-sandbox",
//        "--disable-extensions",
//        "--disable-default-apps",
//        "--disable-component-extensions-with-background-pages",
//        //"--disable-field-trial-config",
//        //"--disable-background-networking",
//        //"--disable-back-forward-cache",
//        "--disable-component-update",
//        //"--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate,HttpsUpgrades,PaintHolding",
//        //"--disable-dev-shm-usage",
//        //"--disable-ipc-flooding-protection",
//        //"--disable-background-timer-throttling",
//        "--force-color-profile=srgb",
//        //"--no-default-browser-check",
//        //"--remote-debugging-pipe",
//    };
//    List<string> ignoreArgs = [
//                "--enable-automation",
//        "--no-sandbox",
//        "--disable-extensions",
//        "--disable-default-apps",
//        "--disable-component-extensions-with-background-pages",
//        "--disable-dev-shm-usage",
//        "--disable-background-networking",
//        "--disable-sync",
//        "--disable-hang-monitor",
//        "--password-store=basic",
//        "--use-mock-keychain",
//        "--disable-popup-blocking",
//        "--disable-prompt-on-repost",
//        "--force-color-profile=srgb",
//        "--disable-features=TranslateUI,BlinkGenPropertyTrees,ImprovedCookieControls,SameSiteByDefaultCookies",
//        "--disable-background-timer-throttling",
//        "--disable-backgrounding-occluded-windows",
//        "--disable-ipc-flooding-protection",
//        "--disable-renderer-backgrounding",
//        "--disable-client-side-phishing-detection",
//        "--metrics-recording-only",
//        "about:blank",
//        //$"--user-data-dir={userDataDirDefault}",
//        //,
//        ////
//        ////"--disable-background-networking",
//        //"--disable-back-forward-cache",
//        //"--disable-component-update",
//        ////"--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate,HttpsUpgrades,PaintHolding",
//        ////"--disable-dev-shm-usage",
//        ////"--disable-ipc-flooding-protection",
//        ////"--disable-background-timer-throttling",
//        //"--force-color-profile=srgb",
//        ////"--no-default-browser-check",
//        ////"--remote-debugging-pipe",
//    ];

//    static readonly IEnumerable<string> chromiumSwitches = new[] {
//        "--disable-field-trial-config", // https://source.chromium.org/chromium/chromium/src/+/main:testing/variations/README.md
//        "--disable-background-networking",
//        "--enable-features=NetworkService,NetworkServiceInProcess",
//        "--disable-background-timer-throttling",
//        "--disable-backgrounding-occluded-windows",
//        "--disable-back-forward-cache", // Avoids surprises like main request not being intercepted during page.goBack().
//        "--disable-breakpad",
//        "--disable-client-side-phishing-detection",
//        "--disable-component-extensions-with-background-pages",
//        "--disable-component-update", // Avoids unneeded network activity after startup.
//        "--no-default-browser-check",
//        "--disable-default-apps",
//        "--disable-dev-shm-usage",
//        "--disable-extensions",
//        // AvoidUnnecessaryBeforeUnloadCheckSync - https://github.com/microsoft/playwright/issues/14047
//        // Translate - https://github.com/microsoft/playwright/issues/16126
//        "--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate",
//        "--allow-pre-commit-input",
//        "--disable-hang-monitor",
//        "--disable-ipc-flooding-protection",
//        "--disable-popup-blocking",
//        "--disable-prompt-on-repost",
//        "--disable-renderer-backgrounding",
//        "--disable-sync",
//        "--force-color-profile=srgb",
//        "--metrics-recording-only",
//        "--no-first-run",
//        "--enable-automation",
//        "--password-store=basic",
//        "--use-mock-keychain",
//        // See https://chromium-review.googlesource.com/c/chromium/src/+/2436773
//        "--no-service-autorun",
//        "--export-tagged-pdf"
//    };

//    public async Task LaunchPersistentContextAsync(string userDataDirDefault, string exepath, IUserProfile profile, string url, string exts)
//    {
//        while (IsBusy)
//            await Task.Delay(500);

//        //if (createdContext.TryGetValue(userDataDirDefault, out IBrowser? value))
//        //{
//        //    if (value is not null &&
//        //        value.Contexts is not null &&
//        //        value.Contexts.Any() &&
//        //        value.Contexts[0].Pages is not null &&
//        //        value.Contexts[0].Pages.Any())
//        //        await value.Contexts[0].Pages[0].BringToFrontAsync();

//        //    return;
//        //}
//        Interlocked.Increment(ref _isBusy);
//        try
//        {
//            var proxyextdir = Path.Combine(userDataDirDefault, "proxyext");
//            if (Directory.Exists(proxyextdir))
//                Directory.Delete(proxyextdir, true);

//            if (profile.Proxy != null &&
//            profile.Proxy.CanUse &&
//            !string.IsNullOrEmpty(profile.Proxy.Host) &&
//            !string.IsNullOrEmpty(profile.Proxy.UserName) &&
//            !string.IsNullOrEmpty(profile.Proxy.Password))
//            {
//                //from：https://github.com/henices/Chrome-proxy-helper
//                var manifest_json = """
//    {
//        "version": "1.0.0",
//        "manifest_version": 2,
//        "name": "Chrome Proxy",
//        "permissions": [
//            "proxy",
//            "tabs",
//            "unlimitedStorage",
//            "storage",
//            "<all_urls>",
//            "webRequest",
//            "webRequestBlocking"
//        ],
//        "background": {
//            "scripts": ["background.js"]
//        },
//        "minimum_chrome_version":"22.0.0"
//    }
//    """;

//                var background_js = """
//                    function callbackFn(details) {
//                    return { authCredentials: {username: 
//                 """ + $"\"{profile.Proxy.UserName}\"," + " password: " + $"\"{profile.Proxy.Password}\"" +
//                    """
//        } };
//            };
        

//        chrome.webRequest.onAuthRequired.addListener(
//                    callbackFn,
//                    {urls: ["<all_urls>"]},
//                    ['blocking']
//        );

//                chrome.proxy.onProxyError.addListener(function(details) {
//            console.log("fatal: ", details.fatal);
//            console.log("error: ", details.error);
//            console.log("details: ", details.details)
//        });
//        """;

//                if (!Directory.Exists(proxyextdir))
//                    Directory.CreateDirectory(proxyextdir);

//                await File.WriteAllTextAsync(Path.Combine(proxyextdir, "manifest.json"), manifest_json);
//                await File.WriteAllTextAsync(Path.Combine(proxyextdir, "background.js"), background_js);
//            }

//            //return plugin_file
//            //IBrowser? browserContext = null;
//            //createdContext.Add(userDataDirDefault, browserContext);
//            //playwright ??= await Playwright.CreateAsync();

//            var port = Netil.NextFreePort(1000);
//            List<string> args =
//                [
//                    $"--user-data-dir=\"{userDataDirDefault}\"",
//                    //"--restore-last-session",
//                    "--profile-directory=Default",
//                    "--ash-no-nudges",
//                    "--disable-domain-reliability",
//                    "--in-process-gpu",

//                    "--no-default-browser-check",
//                    "--no-first-run",
//                    "--disable-field-trial-config",
//                    $"--remote-debugging-port={port}",
//                ];
//            if (profile.Proxy.CanUse && !string.IsNullOrEmpty(profile.Proxy.Host))
//            {
//                args.Add($"--proxy-server={profile.Proxy.Host}:{profile.Proxy.Port}");
//                if (Directory.Exists(proxyextdir))
//                    exts = string.IsNullOrEmpty(exts) ? proxyextdir : $"{exts},{proxyextdir}";
//            }
//            if (!string.IsNullOrEmpty(exts))
//                args.Add($"--load-extension=\"{exts}\"");

//            if (!profile.WebBrowser.WebRTC)
//            {
//                args.Add("--disable-media-stream");
//                args.Add("--disable-webrtc-hw-encoding");
//                args.Add("--disable-webrtc-hw-decoding");
//                args.Add("--webrtc-stun-probe-trial");
//                args.Add("--use-fake-device-for-media-stream");
//                args.Add("--enable-webrtc-hide-local-ips-with-mdns");
//                args.Add("--force-webrtc-ip-handling-policy");
//                args.Add("--enforce-webrtc-ip-permission-check");
//            }

//            if (!profile.WebBrowser.WebGL)
//            {
//                args.Add("--disable-webgl");
//            }

//            if (!profile.WebBrowser.Tracking)
//            {
//                // not disable tracking totally, but disable for hyperlink
//                args.Add("--disable-hyperlink-auditing");
//            }
//            var process = new Process
//            {
//                StartInfo = new ProcessStartInfo
//                {
//                    FileName = exepath,
//                    Arguments = string.Join(" ", args),
//                    UseShellExecute = true,
//                    ErrorDialog = true
//                },
//                EnableRaisingEvents = true
//            };

//            process.Exited += (s, e) =>
//            {
//                createdContext.Remove(userDataDirDefault);
//            };
//            process.Start();
//            //await Task.Delay(1000);
//            //while(process.MainWindowHandle == IntPtr.Zero) 
//            //await Task.Delay(500);

//            //BrowserTypeLaunchPersistentContextOptions options = new()
//            //{
//            //    ExecutablePath = exepath,
//            //    Headless = false,
//            //    ViewportSize = ViewportSize.NoViewport,
//            //    Args = args,
//            //    //IgnoreAllDefaultArgs = true,
//            //    IgnoreDefaultArgs = ignoreArgs,
//            //    Timeout = 10000,
//            //};

//            //if (profile.Proxy.CanUse && profile.Proxy.Host != null)
//            //{
//            //    options.Proxy = new Microsoft.Playwright.Proxy()
//            //    {
//            //        Server = $"{profile.Proxy.Host}:{profile.Proxy.Port}"
//            //    };

//            //    if (profile.Proxy.UserName != null)
//            //        options.Proxy.Username = profile.Proxy.UserName;

//            //    if (profile.Proxy.Password != null)
//            //        options.Proxy.Password = profile.Proxy.Password;
//            //}

//            //await Task.Factory.StartNew(async() =>
//            //{
//            //    while (!process.HasExited)
//            //    {
//            //        var output = await process!.StandardOutput.ReadLineAsync();
//            //        if (process!.HasExited)
//            //        {
//            //            throw new Exception("browser process exited unexpectedly");
//            //        }
//            //        if (output != null && output.Contains("WebView2 initialized"))
//            //        {
//            //            break;
//            //        }
//            //    }
//            //});

//            //browserContext = await playwright.Chromium.ConnectOverCDPAsync($"http://localhost:{port}");//.LaunchPersistentContextAsync(userDataDirDefault, options);


//            ////  while (t.)
//            //if (browserContext != null && browserContext.IsConnected)
//            //{
//            //    //browserContext
//            //    createdContext[userDataDirDefault] = browserContext;

//            //    browserContext.Disconnected += (s, e) =>
//            //    {
//            //        //if (!createdContext.ContainsKey(userDataDirDefault) && IsBusy)
//            //        //    Interlocked.Decrement(ref _isBusy);
//            //        //else
//            //        createdContext.Remove(userDataDirDefault);
//            //    };

//            //    //var nd = await browserContext.NewContextAsync(new()
//            //    //{
//            //    //    Proxy = options.Proxy,
//            //    //});
//            //    var browser = browserContext.Contexts[0];



//            //    browser.WebError += (s, e) => 
//            //    { 
//            //    };
//            //    browser.Dialog += (s, e) =>
//            //    {

//            //    };
//            //    browser.Request += (s, e) =>
//            //    {
//            //        var c = s as IBrowserContext;
//            //    };

//            //    //await browser.AddInitScriptAsync(
//            //    //                    @"chrome.webRequest.onAuthRequired.addListener(
//            //    //function(details) 
//            //    //{
//            //    //var idstr = details.requestId.toString();
//            //    //if(details.isProxy === true){
//            //    //console.log('AUTH - ' + details.requestId);
//            //    //if(!(idstr in calls)){
//            //    //calls[idstr] = 0;
//            //    //}
//            //    //calls[idstr] = calls[idstr] + 1;
//            //    //var retry =  5;
//            //    //if(calls[idstr] >= retry)
//            //    //{
//            //    //lock();
//            //    //chrome.notifications.create(NOTIFICATION_ID, {
//            //    //'type': 'basic',
//            //    //'iconUrl': 'icon_locked_128.png',
//            //    //'title': 'Proxy Auto Auth error',
//            //    //'message': 'A lot of Proxy Authentication requests have been detected. There is probably a mistake in your credentials. For your safety, the extension has been temporary locked. To unlock it, click the save button in the options.',\
//            //    //isClickable': true,
//            //    //'priority': 2
//            //    //}, function(id){ 
//            //    //calls = {};
//            //    //return({
//            //    //cancel : true
//            //    //});" +
//            //    //$"var login = {profile.Proxy.UserName};" +
//            //    //$"var password ={profile.Proxy.Password};" +
//            //    //@"if (login && password && !locked){
//            //    //return({authCredentials : {'username' : login,'password' : password});"
//            //    //                    );
//            //    foreach (var page in browser.Pages)
//            //    {
//            //        //var l = page.GetByText("Sign in");
//            //        //var all = await l.AllAsync();
//            //        // Delete header
//            //        page.Dialog += (_, e) =>
//            //        {
//            //        };
//            //        await page.RouteAsync("**/*", async route => {
//            //            var headers = new Dictionary<string, string>(route.Request.Headers.ToDictionary(x => x.Key, x => x.Value));
//            //            headers.Remove("X-Secret");
//            //            await route.ContinueAsync(new() { Headers = headers });
//            //        });
//            //    }
//            //    //            await browser.AddInitScriptAsync(
//            //    //                @"const defaultGetter = Object.getOwnPropertyDescriptor(
//            //    //  Navigator.prototype,
//            //    //  ""webdriver""
//            //    //).get;
//            //    //defaultGetter.apply(navigator);
//            //    //defaultGetter.toString();
//            //    //Object.defineProperty(Navigator.prototype, ""webdriver"", {
//            //    //  set: undefined,
//            //    //  enumerable: true,
//            //    //  configurable: true,
//            //    //  get: new Proxy(defaultGetter, {
//            //    //    apply: (target, thisArg, args) => {
//            //    //      Reflect.apply(target, thisArg, args);
//            //    //      return false;
//            //    //    },
//            //    //  }),
//            //    //});
//            //    //const patchedGetter = Object.getOwnPropertyDescriptor(
//            //    //  Navigator.prototype,
//            //    //  ""webdriver""
//            //    //).get;
//            //    //patchedGetter.apply(navigator);
//            //    //patchedGetter.toString();"
//            //    //);


//            //    if (url != null)
//            //    {
//            //        if (!url.StartsWith("http://") || !url.StartsWith("https://"))
//            //            url = "http://" + url;
//            //        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri2))
//            //        {
//            //            var page = await browser.NewPageAsync();
//            //            await page.GotoAsync(uri2.AbsoluteUri);
//            //        }
//            //    }
//            //    else
//            //    {
//            //        //var n = await browserContext.NewPageAsync();
//            //    }
//            //}
//            //else
//            //{
//            //    //createdContext.Remove(userDataDirDefault);
//            //}
//        }
//        catch 
//        {
//            createdContext.Remove(userDataDirDefault);
//        }
//        finally
//        {
//        }

//        Interlocked.Decrement(ref _isBusy);
//    }

//    private long _isBusy;
//}
