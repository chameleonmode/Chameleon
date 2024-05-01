using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.SystemBrowser.Browsers;

public abstract class SystemBrowserBase : ISystemBrowser
{
    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
    public Dictionary<int, ISystemBrowserInstance> Instances => instances;


    private long _isBusy;
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;


    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        while (IsBusy)
            await Task.Delay(500);

        Interlocked.Increment(ref _isBusy);
        ISystemBrowserInstance browser;
        try
        {
            if (!Instances.TryGetValue(o.UserProfile.Id, out browser))
            {
                browser = InitializeBrowser(o);
                browser.OnProcessClosed += Browser_OnProcessClosed;
                Instances[o.UserProfile.Id] = browser;
            }

            await browser.Open();
        }
        finally { Interlocked.Decrement(ref _isBusy); }

        return browser;
    }
    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);

    public void Browser_OnProcessClosed(ISystemBrowserLaunchOptions o)
    {
        Instances.Remove(o.UserProfile.Id);
    }
}
