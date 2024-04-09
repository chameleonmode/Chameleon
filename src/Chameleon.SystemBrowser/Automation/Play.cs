using Chameleon.Interfaces.UserProfiles;
using Chameleon.SystemBrowser.Common;
using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Automation;

public class Play
{

    public static Play Instance { get; } = new Play();
    Dictionary<string,  IBrowserContext> createdContext = []; 

        IPlaywright? playwright = null;
    IBrowserType? chromium = null;
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    private Play()
    {
        // Init();
    }

    static readonly IEnumerable<string> ignoreDefaultArgs = new[]
    {
        "about:blank",
        "--enable-automation",
        "--no-sandbox",
        "--disable-extensions",
        "--disable-default-apps",
        "--disable-component-extensions-with-background-pages",
        //"--disable-field-trial-config",
        //"--disable-background-networking",
        //"--disable-back-forward-cache",
        "--disable-component-update",
        //"--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate,HttpsUpgrades,PaintHolding",
        //"--disable-dev-shm-usage",
        //"--disable-ipc-flooding-protection",
        //"--disable-background-timer-throttling",
        "--force-color-profile=srgb",
        //"--no-default-browser-check",
        //"--remote-debugging-pipe",
    };

    static readonly IEnumerable<string> chromiumSwitches = new[] {
        "--disable-field-trial-config", // https://source.chromium.org/chromium/chromium/src/+/main:testing/variations/README.md
        "--disable-background-networking",
        "--enable-features=NetworkService,NetworkServiceInProcess",
        "--disable-background-timer-throttling",
        "--disable-backgrounding-occluded-windows",
        "--disable-back-forward-cache", // Avoids surprises like main request not being intercepted during page.goBack().
        "--disable-breakpad",
        "--disable-client-side-phishing-detection",
        "--disable-component-extensions-with-background-pages",
        "--disable-component-update", // Avoids unneeded network activity after startup.
        "--no-default-browser-check",
        "--disable-default-apps",
        "--disable-dev-shm-usage",
        "--disable-extensions",
        // AvoidUnnecessaryBeforeUnloadCheckSync - https://github.com/microsoft/playwright/issues/14047
        // Translate - https://github.com/microsoft/playwright/issues/16126
        "--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate",
        "--allow-pre-commit-input",
        "--disable-hang-monitor",
        "--disable-ipc-flooding-protection",
        "--disable-popup-blocking",
        "--disable-prompt-on-repost",
        "--disable-renderer-backgrounding",
        "--disable-sync",
        "--force-color-profile=srgb",
        "--metrics-recording-only",
        "--no-first-run",
        "--enable-automation",
        "--password-store=basic",
        "--use-mock-keychain",
        // See https://chromium-review.googlesource.com/c/chromium/src/+/2436773
        "--no-service-autorun",
        "--export-tagged-pdf"
    };

    public async Task LaunchPersistentContextAsync(string userDataDirDefault, string exepath, IUserProfile profile, string url, string exts)
    {
        while (IsBusy)
            await Task.Delay(500);

        if (createdContext.ContainsKey(userDataDirDefault))
        {
            if (createdContext[userDataDirDefault].Pages.Any())
                await createdContext[userDataDirDefault].Pages.Last().BringToFrontAsync();

            return;
        }

        Interlocked.Increment(ref _isBusy);
        IBrowserContext? browser = null;
        try
        {
            playwright ??= await Playwright.CreateAsync();
            chromium ??= playwright.Chromium;


            BrowserTypeLaunchPersistentContextOptions options = new()
            {
                ExecutablePath = exepath,
                Headless = false,
                ViewportSize = ViewportSize.NoViewport,

                //IgnoreAllDefaultArgs = true,
                IgnoreDefaultArgs = new[]
                {
                    "about:blank",
                    "--enable-automation",
                    "--no-sandbox",
                    "--disable-extensions",
                    "--disable-default-apps",
                    "--disable-component-extensions-with-background-pages",
                    //"--disable-field-trial-config",
                    //"--disable-background-networking",
                    "--disable-back-forward-cache",
                    "--disable-component-update",
                    //"--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate,HttpsUpgrades,PaintHolding",
                    //"--disable-dev-shm-usage",
                    //"--disable-ipc-flooding-protection",
                    //"--disable-background-timer-throttling",
                    "--force-color-profile=srgb",
                    //"--no-default-browser-check",
                    //"--remote-debugging-pipe",
                },
            };

            List<string> args = [
                "--ash-no-nudges",
                "--disable-domain-reliability",
                //"--no-default-browser-check",
                $"--load-extension={exts}",
                $"--remote-debugging-port={SystemBrowserInstance.NextFreePort(1000)}"];
            if (!profile.WebBrowser.WebRTC)
            {
                args.Add("--disable-media-stream");
                args.Add("--disable-webrtc-hw-encoding");
                args.Add("--disable-webrtc-hw-decoding");
                args.Add("--webrtc-stun-probe-trial");
                args.Add("--use-fake-device-for-media-stream");
                args.Add("--enable-webrtc-hide-local-ips-with-mdns");
                args.Add("--force-webrtc-ip-handling-policy");
                args.Add("--enforce-webrtc-ip-permission-check");
            }

            if (!profile.WebBrowser.WebGL)
            {
                args.Add("--disable-webgl");
            }

            if (!profile.WebBrowser.Tracking)
            {
                // not disable tracking totally, but disable for hyperlink
                args.Add("--disable-hyperlink-auditing");
            }
            options.Args = args;

            if (profile.Proxy.CanUse && profile.Proxy.Host != null)
            {
                options.Proxy = new Microsoft.Playwright.Proxy()
                {
                    Server = $"{profile.Proxy.Host}:{profile.Proxy.Port}"
                };

                if (profile.Proxy.UserName != null)
                    options.Proxy.Username = profile.Proxy.UserName;

                if (profile.Proxy.Password != null)
                    options.Proxy.Password = profile.Proxy.Password;
            }

            browser = await chromium.LaunchPersistentContextAsync(userDataDirDefault, options);

            await browser.AddInitScriptAsync(
                @"const defaultGetter = Object.getOwnPropertyDescriptor(
      Navigator.prototype,
      ""webdriver""
    ).get;
    defaultGetter.apply(navigator);
    defaultGetter.toString();
    Object.defineProperty(Navigator.prototype, ""webdriver"", {
      set: undefined,
      enumerable: true,
      configurable: true,
      get: new Proxy(defaultGetter, {
        apply: (target, thisArg, args) => {
          Reflect.apply(target, thisArg, args);
          return false;
        },
      }),
    });
    const patchedGetter = Object.getOwnPropertyDescriptor(
      Navigator.prototype,
      ""webdriver""
    ).get;
    patchedGetter.apply(navigator);
    patchedGetter.toString();"
);
            browser.Close += async (s, e) =>
            {
                //await createdContext[userDataDirDefault].DisposeAsync();
                createdContext.Remove(userDataDirDefault);
            };

            if (url != null)
            {
                if(!url.StartsWith("http://") || !url.StartsWith("https://"))
                    url = "http://" + url;
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri2))
                {
                    var page = await browser.NewPageAsync();
                    await page.GotoAsync(uri2.AbsoluteUri);
                }
            }
        }
        catch(Exception ex) 
        {

        }
        finally
        {
            if (browser is not null)
                createdContext.Add(userDataDirDefault, browser);

            Interlocked.Decrement(ref _isBusy);
        }
    }

    private long _isBusy;
}
