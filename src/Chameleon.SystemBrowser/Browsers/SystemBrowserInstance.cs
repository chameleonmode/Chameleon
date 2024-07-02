namespace Chameleon.SystemBrowser.Common;

public abstract class SystemBrowserInstance(
    IEventAggregator eventAggregator,
    ISystemBrowserLaunchOptions options,
    IUserDefaultSettingsService userDefaultsSettingsService,
    string browserDataFolderPath,
    string browserExeFilePath)
    : ISystemBrowserInstance
{
    public event Action<ISystemBrowserLaunchOptions> OnProcessClosed;

    private readonly string pexdir = Guid.NewGuid().ToString();
    private readonly List<IntPtr> winEventHooks = [];

    public string BrowserExeFilePath => browserExeFilePath;

    private U32.WinEventDelegate winEventsCaptureDelegate;
    private MWHandleTrackerUtility windowTracker;

    public TaskCompletionSource<bool> OPtcs { get; } = new();

    public virtual async Task Open()
    {
        if (Brocess is null || Handle == IntPtr.Zero)
        {
            Starturl = await userDefaultsSettingsService.GetRandomUrlAsync();
            Port = Netil.NextFreePort(9613);

            await EnsureProfileFolderCreated();
            await InitializeProfileFolder();
            await InitializeExtensionPath();
            await StartProcess();
        }

        MakeForeground();
    }

    public void MakeForeground()
    {
        if (Brocess != null)
        {
            if (!IsMao)
            {
                if (Handle == IntPtr.Zero)
                    return;

#pragma warning disable CA1416 // Validate platform compatibility
                if (U32.IsWindow(Handle))
                {
                    U32.SetForegroundWindow(Handle);
                    U32.SetActiveWindow(Handle);
                }
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else
            {
                MacOSUtil.SetForegroundWindow(Brocess.Id);
            }
        }
    }

    protected virtual async Task InitializeExtensionPath()
    {
        await IOtil.DeleteDExistsAsync(ProxyAddonUtil.ProxyExtDir(BrowserProfileFolderPath));

        if (HasProxyLogin)
        {
            await IOtil.CreateDirectory(ProxyExtDir);

            await IOtil.WriteTextToFileAsync(Path.Combine(ProxyExtDir, "manifest.json"), ProxyAddonUtil.GetManifestv3());
            await IOtil.WriteTextToFileAsync(Path.Combine(ProxyExtDir, "background.js"), ProxyAddonUtil.GetBgJsv3(UserProfile.Proxy));
        }
    }

    protected virtual async Task StartProcess()
    {
        Brocess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = BrowserExeFilePath,
                Arguments = GetCommandLineArguments(),
                UseShellExecute = true,
                ErrorDialog = true,
            },
            EnableRaisingEvents = true,
        };
        Brocess.Start();



        if (IsMao)
        {
            Handle = Brocess.Handle;
            Brocess.Exited += (s,e)=> { Cleanup(); };
        }
        else
        {
#pragma warning disable CA1416 // Validate platform compatibility
            windowTracker = new(Brocess);
            var newHandle = await windowTracker.WaitForMainWindowHandleChangeAsync();
            Brocess = newHandle.Item2;
            if (Brocess == null)
            {
                OPtcs.TrySetResult(false);
            }
            else
            {
                Handle = Brocess.MainWindowHandle;
                windowTracker.StopTracking();
                SetWin32Events();
            }
#pragma warning restore CA1416 // Validate platform compatibility
        }

        OPtcs.TrySetResult(true);
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private void SetWin32Events()
    {
        if (Brocess != null && Handle != IntPtr.Zero)
        {
            winEventsCaptureDelegate = WinEventProc;

            // capture EVENT_OBJECT_FOCUS
            winEventHooks.Add(U32.SetWinEventHook(
                User32Events.EVENT_OBJECT_FOCUS,
                User32Events.EVENT_OBJECT_FOCUS,
                IntPtr.Zero,
                winEventsCaptureDelegate,
                (uint)Brocess.Id,
                0,
                (uint)User32Events.WINEVENT_OUTOFCONTEXT));

            //capture window close
            winEventHooks.Add(U32.SetWinEventHook(
                User32Events.EVENT_OBJECT_DESTROY,
                User32Events.EVENT_OBJECT_DESTROY,
                IntPtr.Zero,
                winEventsCaptureDelegate,
                0,
                0,
                (uint)User32Events.WINEVENT_OUTOFCONTEXT));

            U32.SetForegroundWindow(Handle);
            U32.SetActiveWindow(Handle);
        }
    }

    private async void WinEventProc(IntPtr hWinEventHook, User32Events eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        switch (eventType)
        {
            case User32Events.EVENT_OBJECT_FOCUS:
                if (hwnd == Handle)
                {
                    eventAggregator
                        .GetEvent<ForegroundUserSystemBrowserEvent>()
                        .Publish(GetArgs(Brocess));
                }
                break;
            case User32Events.EVENT_SYSTEM_MENUSTART:
            case User32Events.EVENT_SYSTEM_MENUEND:
                if (idObject == 0 || idObject == -1)
                {
                    //TODO:
                    //context sensitive menu
                }
                break;
            case User32Events.EVENT_SYSTEM_MINIMIZEEND:
            case User32Events.EVENT_SYSTEM_MINIMIZESTART:
            case User32Events.EVENT_SYSTEM_MOVESIZEEND:
                // only care about child windows that are moved by user
                break;

            case User32Events.EVENT_OBJECT_DESTROY:
                _ = await OPtcs.Task;

                if (Handle == IntPtr.Zero || Brocess == null || Brocess.HasExited)
                    Cleanup();
                break;

            default:
                break;
        }
    }

    protected virtual void Cleanup()
    {
        ExUtil.TryCatch(() =>
        {
            if (!IsMao)
                foreach (var item in winEventHooks)
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    U32.UnhookWinEvent(item);
#pragma warning restore CA1416 // Validate platform compatibility
                }
        });

        var r = OPtcs.TrySetResult(false);
        Brocess = null;
        Handle = IntPtr.Zero;
        OnProcessClosed?.Invoke(options);
    }

    public UserProfileSystemBrowserProcessEventArgs GetArgs(Process process) => new UserProfileSystemBrowserProcessEventArgs(
                UserProfile,
                BrowserType,
                process,
                options.Url,
                options.SignIn
                );

    protected async Task EnsureProfileFolderCreated()
    {
        await IOtil.CreateDirectory(BrowserProfileFolderPath);
        await OnProfileFolderCreated();
    }

    protected virtual Task OnProfileFolderCreated()
    {
        return Task.CompletedTask;
    }

    protected virtual Task InitializeProfileFolder()
    {
        return Task.CompletedTask;
    }

    protected virtual List<string> GetClearCommandLineArgumentsList()
    {
        List<string> args =
            [
                "--restore-last-session",
                "--profile-directory=Default",
                "--ash-no-nudges",
                "--disable-domain-reliability",
                "--in-process-gpu",
                "--no-default-browser-check",
                "--no-first-run",
                "--disable-field-trial-config",
                "--disable-software-rasterizer",
                $"--remote-debugging-port={Port}",
                $"--window-name=\"{UserProfile.Title}\"", 
            ];

        if (UserProfile.Proxy?.CanUse == true && UserProfile.Proxy.Host.HasAny())
        {
            args.Add($"--proxy-server=http://{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}");
        }

        if (!UserProfile.WebBrowser.WebRTC)
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

        if (!UserProfile.WebBrowser.WebGL)
        {
            args.Add("--disable-webgl");
        }

        if (!UserProfile.WebBrowser.Tracking)
        {
            // not disable tracking totally, but disable for hyperlink
            args.Add("--disable-hyperlink-auditing");
        }

        return args;
    }

    protected virtual List<string> GetCommandLineArgumentsList()
    {
        var args = GetClearCommandLineArgumentsList();

        if (GetLoadExtensionsArgument().Get() is string exts)
            args.Add($"--load-extension=\"{exts}\"");

        args.Add($"--user-data-dir=\"{BrowserProfileFolderPath}\"");
        args.Add(Starturl);

        return args;
    }

    protected virtual string GetCommandLineArguments()
    {
        IEnumerable<string> args = GetCommandLineArgumentsList();
        return string.Join(" ", args);
    }

    public virtual string GetLoadExtensionsArgument()
    {
        List<string> exts = [];
        if (Directory.Exists(ProxyExtDir))
            exts.Add(ProxyExtDir);

        if (Directory.Exists(BrowserExtensionsFolderPath))
            exts.AddRange(Directory.GetDirectories(BrowserExtensionsFolderPath));

        return exts.ToCommaSeparatedString();
    }

    public string BrowserProfileFolderPath =>
    Path.Combine(browserDataFolderPath, BrowserType.ToString(), UserProfile.Id.ToString());

    protected string BrowserExtensionsFolderPath =>
        Path.Combine(AddonsUtil.BrowserExtensionsRootFolderPath, BrowserType.ToString());

    public string ProxyExtDir =>
        Path.Combine(ProxyAddonUtil.ProxyExtDir(BrowserProfileFolderPath), pexdir);

    public IUserProfile UserProfile =>
        options.UserProfile;

    public static bool IsMao =>
        OperatingSystem.IsMacOS();

    public bool HasProxyLogin =>
        UserProfile.Proxy?.CanUse == true &&
        UserProfile.Proxy.Host.HasAny() &&
        UserProfile.Proxy.UserName.HasAny() &&
        UserProfile.Proxy.Password.HasAny();

    public string Starturl { get; private set; }
    public int Port { get; private set; }
    public Process? Brocess { get; private set; }
    public IntPtr Handle { get; private set; } = IntPtr.Zero;
    protected abstract SystemBrowserType BrowserType { get; }
}

