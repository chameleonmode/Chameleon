using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace Chameleon.SystemBrowser.Browsers;
public abstract class SystemBrowserBase(
    IEventAggregator eventAggregator,
    IApplicationEnvironment applicationEnvironment,
    ISetPreferencesService setPreferencesService,
    IUserDefaultSettingsService userDefaultsSettingsService)
    : ISystemBrowser
{
    //private System.Timers.Timer _pollingTimer;
    private readonly SemaphoreSlim _openSemaphore = new(1, 1);
    private readonly List<int> pollingids = [];
    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
    private long _isBusy;

    protected IEventAggregator EventAggregator { get; } = eventAggregator;
    protected IApplicationEnvironment ApplicationEnvironment { get; } = applicationEnvironment;
    protected ISetPreferencesService SetPreferencesService { get; } = setPreferencesService;
    protected IUserDefaultSettingsService UserDefaultSettingsService { get; } = userDefaultsSettingsService;

    public string Path => GetSystemBrowserExePath();
    public string ChamelonPath => GetBrowserExePath();
    public string Directory => GetDirectoryPath();
    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;
    public Dictionary<int, ISystemBrowserInstance> Instances => instances;

    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
    {
        if (!Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
        {
            await TaskUtil.AwaitFor(() => !IsBusy, 36, 350);
            //if (!OperatingSystem.IsMacOS() && _pollingTimer == null)
            //{
            //    _pollingTimer = new System.Timers.Timer(5000);
            //    _pollingTimer.Start();
            //    _pollingTimer.Elapsed += OnPollingTimerElapsed;
            //}
            Interlocked.Increment(ref _isBusy);
            try
            {
                browser = await InitializeBrowserAsync(o);
                browser.OnProcessClosed += Browser_OnProcessClosed;
                Instances[o.UserProfile.Id] = browser;

                _ = browser.Open();

                if (await browser.OPtcs.Task)
                {
                    var args = browser.GetArgs;
                    EventAggregator.GetEvent<ForegroundUserSystemBrowserEvent>().Publish(args);
                    EventAggregator.GetEvent<OpenedUserSystemBrowserEvent>().Publish(args);
                }
            }
            catch (Exception e)
            {
                await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _isBusy, 0);
            }
        }
        else
        {
            //_ = await browser.OPtcs.Task;

            if (browser.Brocess?.HasExited == true)
            {
                browser.Cleanup();
                await Task.Delay(250);
                _ = Open(o);
            }
            else
                browser.MakeForeground();
        }

        return browser;
    }

    public virtual Task<ISystemBrowserInstance> InitializeBrowserAsync(ISystemBrowserLaunchOptions o) =>
        Task.Run(() => InitializeBrowser(o));

    private async void OnPollingTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        for (int i = Instances.Count - 1; i >= 0; i--)
        {
            var uid = Instances.Keys.ElementAt(i);
            if (Instances.TryGetValue(uid, out ISystemBrowserInstance browser))
            {
                _ = await browser.OPtcs.Task;

                if (browser.Brocess?.HasExited == true)
                    browser.Cleanup();
            }
        }
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

            await Task.Delay(250);
        }
        while (IsBusy);
    }

    protected abstract string GetBrowserExePath();
    protected abstract string GetSystemBrowserExePath();
    protected abstract string GetDirectoryPath();
    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);
    public abstract SystemBrowserType BrowserType { get; }
}

//public abstract class SystemBrowserBase(
//    IEventAggregator eventAggregator,
//    IApplicationEnvironment applicationEnvironment,
//    ISetPreferencesService setPreferencesService,
//    IUserDefaultSettingsService userDefaultsSettingsService)
//    : ISystemBrowser
//{
//    protected IEventAggregator EventAggregator { get; } = eventAggregator;
//    protected IApplicationEnvironment ApplicationEnvironment { get; } = applicationEnvironment;
//    protected ISetPreferencesService SetPreferencesService { get; } = setPreferencesService;
//    protected IUserDefaultSettingsService UserDefaultSettingsService { get; } = userDefaultsSettingsService;

//    private readonly ConcurrentBag<int> tsUserProfileIds = [];
//    private readonly SemaphoreSlim _semaphore = new(1, 1);
//    private readonly object _lock = new();

//    private long _isBusy;
//    private System.Timers.Timer _pollingTimer;

//    private readonly Dictionary<int, ISystemBrowserInstance> instances = [];
//    public Dictionary<int, ISystemBrowserInstance> Instances => instances;

//    public string Path => GetSystemBrowserExePath();
//    public string ChamelonPath => GetBrowserExePath();
//    public string Directory => GetDirectoryPath();
//    public bool IsBusy => Interlocked.Read(ref _isBusy) > 0;

//    public virtual async Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions o)
//    {
//        // Thread-safe check before waiting for the semaphore
//        //lock (_lock)
//        //{

//        //}

//        await _semaphore.WaitAsync();
//        try
//        {
//            Interlocked.Increment(ref _isBusy);
//            if (!OperatingSystem.IsMacOS() && _pollingTimer == null)
//            {
//                _pollingTimer = new System.Timers.Timer(5000);
//                _pollingTimer.Elapsed += OnPollingTimerElapsed;
//                _pollingTimer.Start();
//            }

//            if (Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
//            {
//                browser.MakeForeground();
//                return browser;
//            }
//            browser = await InitializeBrowserAsync(o);
//            browser.OnProcessClosed += Browser_OnProcessClosed;
//            _ = browser.Open();

//            if (await browser.OPtcs.Task)
//            {
//                tsUserProfileIds.Add(o.UserProfile.Id);
//                Instances[o.UserProfile.Id] = browser;
//                var args = browser.GetArgs;
//                EventAggregator.GetEvent<ForegroundUserSystemBrowserEvent>().Publish(args);
//                EventAggregator.GetEvent<OpenedUserSystemBrowserEvent>().Publish(args);
//            }

//        }
//        catch (Exception e)
//        {
//            await MesageBoxHelper.ShowErrorAsync("Error", e.Message);
//        }
//        finally
//        {
//            Interlocked.Exchange(ref _isBusy, 0);
//            _semaphore.Release();
//        }

//        return null;
//    }

//    public virtual Task<ISystemBrowserInstance> InitializeBrowserAsync(ISystemBrowserLaunchOptions o) =>
//        Task.Run(() => InitializeBrowser(o));

//    private async void OnPollingTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
//    {
//        //await _semaphore.WaitAsync();
//        //try
//        //{
//            //lock (_lock)
//            //{
//            for (int i = Instances.Count - 1; i >= 0; i--)
//            {
//                //var uid = tsUserProfileIds.ElementAt(i);
//                var key = Instances.Keys.ElementAt(i);
//                if (Instances.TryGetValue(key, out ISystemBrowserInstance browser))
//                {
//                    _ = await browser.OPtcs.Task;
//                    if (browser.Brocess?.HasExited == true)
//                    {
//                        //tsUserProfileIds.TryTake(out uid);
//                        browser.Cleanup();
//                    }
//                }
//            }
//        //}
//        ////}
//        //finally
//        //{
//        //    _semaphore.Release();
//        //}
//    }
//    public async void Browser_OnProcessClosed(ISystemBrowserLaunchOptions o)
//    {
//        do
//        {
//            if (Instances.TryGetValue(o.UserProfile.Id, out ISystemBrowserInstance browser))
//            {
//                _ = await browser.OPtcs.Task;
//                if (browser.OPtcs.Task.IsCompleted && browser.Brocess?.HasExited == true)
//                {
//                    EventAggregator
//                        .GetEvent<ClosedUserSystemBrowserEvent>()
//                        .Publish(browser.GetArgs);

//                    Instances.Remove(o.UserProfile.Id);
//                }
//            }
//            else
//                await Task.Delay(250);
//        } while (IsBusy);
//    }


//    protected abstract string GetBrowserExePath();
//    protected abstract string GetSystemBrowserExePath();
//    protected abstract string GetDirectoryPath();
//    public abstract ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o);
//    public abstract SystemBrowserType BrowserType { get; }
//}
