namespace Chameleon.SystemBrowser.Browsers;

public abstract class SystemBrowserBase(
    IEventAggregator eventAggregator,
    IApplicationEnvironment applicationEnvironment,
    ISetPreferencesService setPreferencesService,
    IUserDefaultSettingsService userDefaultsSettingsService)
    : ISystemBrowser
{
    private System.Timers.Timer _pollingTimer;
    private readonly List<int> pollingids = [];

    protected IEventAggregator EventAggregator { get; } = eventAggregator;
    protected IApplicationEnvironment ApplicationEnvironment { get; } = applicationEnvironment;
    protected ISetPreferencesService SetPreferencesService { get; } = setPreferencesService;
    protected IUserDefaultSettingsService UserDefaultSettingsService { get; } = userDefaultsSettingsService;

    public string Path => GetSystemBrowserExePath();
    public string ChamelonPath => GetBrowserExePath();
    public string Directory => GetDirectoryPath();
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    public Dictionary<int, ISystemBrowserInstance> Instances => instances;


    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
    private long _isBusy;

    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        await TaskUtil.AwaitFor(()=>!IsBusy, 120, 500);
        if(!OperatingSystem.IsMacOS() && _pollingTimer == null)
        {
            _pollingTimer = new(1000);
            _pollingTimer.Elapsed -= OnPollingTimerElapsed;
            _pollingTimer.Elapsed += OnPollingTimerElapsed;
        }
        if (!Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
            try
            {
                Interlocked.Increment(ref _isBusy);
                browser = await InitializeBrowserAsync(o);
                browser.OnProcessClosed += Browser_OnProcessClosed;
                Instances[o.UserProfile.Id] = browser;


                _ = browser.Open();

                var opened = await browser.OPtcs.Task;
                if (opened)
                {
                    EventAggregator
                        .GetEvent<ForegroundUserSystemBrowserEvent>()
                        .Publish(browser.GetArgs);

                    EventAggregator
                        .GetEvent<OpenedUserSystemBrowserEvent>()
                        .Publish(browser.GetArgs);
                }
            }
            catch (Exception e)
            {
                await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
            }
            finally { Interlocked.Exchange(ref _isBusy, 0); }
        else
            await browser.MakeForeground();

        return browser;
    }
    public virtual Task<ISystemBrowserInstance> InitializeBrowserAsync(ISystemBrowserLaunchOptions o) =>
        Task.Run(() => InitializeBrowser(o));

    private void OnPollingTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        ExUtil.TryCatch(()=>
        {
            for(int i = Instances.Count; i <= 0; i--)
            {
                var b = Instances.ElementAt(i).Value;
                if(b.Brocess?.HasExited == true)
                    b.Cleanup();
            }
        });
    }
    public async void Browser_OnProcessClosed(ISystemBrowserLaunchOptions o)
    {
        do
        {
            if (Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
            {
                _ = await browser.OPtcs.Task;

                EventAggregator
                   .GetEvent<ClosedUserSystemBrowserEvent>()
                   .Publish(browser.GetArgs);

                Instances.Remove(o.UserProfile.Id);

                break;
            }

            await Task.Delay(500);
        }
        while (IsBusy);
    }


    protected abstract string GetBrowserExePath();
    protected abstract string GetSystemBrowserExePath();
    protected abstract string GetDirectoryPath();
    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);
    public abstract SystemBrowserType BrowserType { get; }
}
