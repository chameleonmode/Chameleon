namespace Chameleon.SystemBrowser.Browsers;

public abstract class SystemBrowserBase(IEventAggregator eventAggregator) : ISystemBrowser
{
    protected IEventAggregator EventAggregator { get; } = eventAggregator;

    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
    private long _isBusy;

    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        while (IsBusy)
            await Task.Delay(500);

        if (!Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
            try
            {
                Interlocked.Increment(ref _isBusy);
                browser = await Task.Run(() => InitializeBrowser(o));
                browser.OnProcessClosed += Browser_OnProcessClosed;
                Instances[o.UserProfile.Id] = browser;


                _ = browser.Open();

                var opened = await browser.OPtcs.Task;
                if (opened)
                {
                    EventAggregator
                        .GetEvent<ForegroundUserSystemBrowserEvent>()
                        .Publish(browser.GetArgs(null));

                    EventAggregator
                        .GetEvent<OpenedUserSystemBrowserEvent>()
                        .Publish(browser.GetArgs(null));
                }
            }
            catch (Exception e)
            {
                await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
            }
            finally { Interlocked.Decrement(ref _isBusy); }
        else
            browser.MakeForeground();

        return browser;
    }
    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);

    public async void Browser_OnProcessClosed(ISystemBrowserLaunchOptions o)
    {
        do
        {
            if (Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
            {
                _ = await browser.OPtcs.Task;

                EventAggregator
                   .GetEvent<ClosedUserSystemBrowserEvent>()
                   .Publish(browser.GetArgs(null));

                Instances.Remove(o.UserProfile.Id);

                break;
            }

            await Task.Delay(500);
        }
        while (IsBusy);
    }

    public static bool IsMao => OperatingSystem.IsMacOS();
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    public Dictionary<int, ISystemBrowserInstance> Instances => instances;
}
