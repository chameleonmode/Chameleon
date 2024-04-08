using Chameleon.Interfaces.UserProfiles;
using Chameleon.SystemBrowser.Common;
using Microsoft.Playwright;

namespace Chameleon.SystemBrowser.Automation;

public class Play
{

    public static Play Instance { get; } = new Play();
    Dictionary<string, Tuple<IPlaywright, IBrowserContext>> createdContext = [];
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    private Play()
    {
        // Init();
    }

    static readonly IEnumerable<string> ignoreDefaultArgs = new[]
    {
        "--enable-automation",
        "--no-sandbox",
        "--disable-extensions",
        "--disable-default-apps",
        "--disable-component-extensions-with-background-pages",
        //"--disable-field-trial-config",
        "--disable-background-networking",
        "--disable-back-forward-cache",
        "--disable-component-update",
        "--disable-features=ImprovedCookieControls,LazyFrameLoading,GlobalMediaControls,DestroyProfileOnBrowserClose,MediaRouter,DialMediaRouteProvider,AcceptCHFrame,AutoExpandDetailsElement,CertificateTransparencyComponentUpdater,AvoidUnnecessaryBeforeUnloadCheckSync,Translate,HttpsUpgrades,PaintHolding",
        //"--disable-dev-shm-usage",
        "--disable-ipc-flooding-protection",
        "--disable-background-timer-throttling",
        "--force-color-profile=srgb",
        "--no-default-browser-check",
        //"--remote-debugging-pipe",
    };

    public async Task LaunchPersistentContextAsync(string userDataDirDefault, string exepath, IUserProfile profile, string url, string exts)
    {
        while (IsBusy)
            await Task.Delay(500);

        if (createdContext.ContainsKey(userDataDirDefault))
            return;

        Interlocked.Increment(ref _isBusy);

        try
        {

            var playwright = await Playwright.CreateAsync();
            var chromium = playwright.Chromium;


            BrowserTypeLaunchPersistentContextOptions options = new()
            {
                ExecutablePath = exepath,
                Headless = false,
                ViewportSize = ViewportSize.NoViewport,
                 
                //IgnoreAllDefaultArgs = true,
                IgnoreDefaultArgs = ignoreDefaultArgs,
            };

            List<string> args = [
                "--no-default-browser-check",
                $"--load-extension={exts}",
                $"--remote-debugging-port={SystemBrowserInstance.NextFreePort(1000)}" ];
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
                args.Add("--disable-webgl ");
            }

            if (!profile.WebBrowser.Tracking)
            {
                // not disable tracking totally, but disable for hyperlink
                args.Add("--disable-hyperlink-auditing ");
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

            var browser = await chromium.LaunchPersistentContextAsync(userDataDirDefault, options);

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
            if (url != null)
            {
                var page = await browser.NewPageAsync();
                await page.GotoAsync(url);
            }

            createdContext.Add(userDataDirDefault, new Tuple<IPlaywright, IBrowserContext>(playwright, browser));
            browser.Close += (s, e) =>
            {
                createdContext.Remove(userDataDirDefault);
            };
        }
        finally
        {
            Interlocked.Decrement(ref _isBusy);
        }
    }

    private long _isBusy;
}
