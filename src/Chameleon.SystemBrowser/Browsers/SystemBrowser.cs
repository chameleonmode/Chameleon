namespace Chameleon.SystemBrowser.Browsers;

public abstract class SystemBrowserBase : ISystemBrowser
{
    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
    private long _isBusy;

    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        while (IsBusy)
            await Task.Delay(500);

        Interlocked.Increment(ref _isBusy);
        ISystemBrowserInstance browser = null;
        try
        {
            if (!Instances.TryGetValue(o.UserProfile.Id, out browser))
            {
                browser = await Task.Run(() => InitializeBrowser(o));
                browser.OnProcessClosed += Browser_OnProcessClosed;
                Instances[o.UserProfile.Id] = browser;
            }

            await browser.Open();
        }
        catch(Exception e)
        {
            await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
        }
        finally { Interlocked.Decrement(ref _isBusy); }

        return browser;
    }
    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);

    public void Browser_OnProcessClosed(ISystemBrowserLaunchOptions o)
    {
        Instances.Remove(o.UserProfile.Id);
    }

    public bool IsMao => OperatingSystem.IsMacOS();
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    public Dictionary<int, ISystemBrowserInstance> Instances => instances;
}
