using Chameleon.CT.Common.Models;
using Chameleon.Interfaces.Settings;
using Chameleon.ThirdParty.GeoIp;
using Chameleon.ThirdParty.GeoIp.Models;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;

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
    public event Action<ISystemBrowserLaunchOptions> OnProcessOpenError;

    private readonly List<IntPtr> winEventHooks = [];
    private List<string> extensions;
    private List<string> Extensions
    {
        get
        {
            if(extensions == null)
                extensions = new()
                {
                    { ProxyExtMainDir },
                    { NavigatorExtMainDir },
                    { TzExtMainDir },
                    { WebRTCExtMainDir },
                    { FontDefenderExtMainDir},
                    { GeoExtMainDir }
                };

            return extensions;
        }
    }


    private U32.WinEventDelegate winEventsCaptureDelegate;

    public TaskCompletionSource<bool> OPtcs { get; } = new();
    protected abstract SystemBrowserType BrowserType { get; }

    public string Starturl { get; private set; }
    public int Port { get; private set; }
    public Process? Brocess { get; set; }
    public IntPtr Handle { get; private set; } = IntPtr.Zero;

    public string BrowserExeFilePath => 
        browserExeFilePath;

    public string BrowserProfileFolderPath =>
        Path.Combine(browserDataFolderPath, BrowserType.ToString(), UserProfile.Id.ToString());


    protected string BrowserExtensionsFolderPath =>
        Path.Combine(AddonsUtil.BrowserExtensionsRootFolderPath, BrowserType.ToString());


    private readonly string exdir_id = Guid.NewGuid().ToString();
    public string BrowserProfileAddonsDir =>
        Path.Combine(BrowserProfileFolderPath, "Chameleon-addons");
    public string ProxyExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, ProxyAddonUtil.AutoProxyFolderName);

    public string NavigatorExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, NavigatorAddon.DirName);

    public string TzExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, TimezoneAddon.DirName);

    public string WebRTCExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, WebRtcAddon.DirName);

    public string FontDefenderExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, FontDefenderAddon.DirName);

    public string GeoExtMainDir =>
        Path.Combine(BrowserProfileAddonsDir, GeoAddon.DirName);


    public IUserProfile UserProfile =>
        options.UserProfile;

    public bool IsRunning => Brocess?.HasExited == false;

    public static bool IsMao =>
        OperatingSystem.IsMacOS();

    public UserProfileSystemBrowserProcessEventArgs GetArgs => 
        new (UserProfile,
            BrowserType,
            Brocess,
            options.Url,
            options.SignIn);

    public bool HasProxyLogin =>
        UserProfile.Proxy?.CanUse == true &&
        UserProfile.Proxy.Host.HasAny() &&
        UserProfile.Proxy.UserName.HasAny() &&
        UserProfile.Proxy.Password.HasAny();

    public virtual async Task Open()
    {
        if (Brocess is null || Handle == IntPtr.Zero)
        {
            Starturl = await userDefaultsSettingsService.GetRandomUrlAsync();
            Port = Netil.NextFreePort(9613);

            await EnsureProfileFolderCreated();
            await InitializeProfileFolder();
            await InitializeExtensionPath();
            if(OPtcs.Task.IsCompleted) 
                return;
            await StartProcess();
        }

        //MakeForeground();
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
                if(MacOSUtil.SetForegroundWindow(Brocess.Id))
                {
                    //Brocess.EnableRaisingEvents = false;
                    //Brocess.Exited -= OnProcessExited; 
                    Brocess.Refresh();
                    //Brocess.Exited += OnProcessExited; 
                    //Brocess.EnableRaisingEvents = true;
                    //await Process.Start(BrowserExeFilePath, GetCommandLineArgumentsList()).WaitForExitAsync();
                    eventAggregator.Pub<ForegroundUserSystemBrowserEvent>(GetArgs);
                }
            }
        }
    }

    protected virtual async Task InitializeExtensionPath()
    {
        await IOtil.DC(BrowserProfileAddonsDir);

        //TODO edit for ff
        if (BrowserType != SystemBrowserType.Firefox)
        {
            var theseOptions = await BrowserDefaultLaunchSettings.Instance();

            if (theseOptions.Options.AutoTimezone && UserProfile.Proxy != null && UserProfile.Proxy.Server.HasAny())
            {
                await ExUtil.AsyncTryCatch(async () =>
                {
                    string ipLookup = await GeoIpApi.Instance.GetIPApi(
                        UserProfile.Proxy.ServerForRequest,
                        onretry => { ToasterHelper.ShowErr(onretry); },
                        UserProfile.Proxy.UserName, UserProfile.Proxy.Password)
                    .ConfigureAwait(false);
                    await TimezoneAddon.InitializeExtension(TzExtMainDir, ipLookup);
                    //ipLookup = await GeoIpApi.Instance.GetGeoIp($"http://{UserProfile.Proxy.Server}", UserProfile.Proxy.UserName, UserProfile.Proxy.Password).ConfigureAwait(false);
                }, (e) =>
                {
                    ToasterHelper.ShowErr($"Request for timezone failed {UserProfile.Proxy.Server} - {e.Message}");
                    OnProcessOpenError?.Invoke((ISystemBrowserLaunchOptions)theseOptions);
                    eventAggregator.Pub<OpenedUserSystemBrowserErrorEvent>(GetArgs);
                    Cleanup();
                });
            }

            if (theseOptions.Options.SpoofGeoLocation)
            {
                await GeoAddon.InitializeExtension(GeoExtMainDir);
            }

            if (BrowserType == SystemBrowserType.Chrome)
            {
                if (theseOptions.Options.DisableWebRTC)
                    await WebRtcAddon.InitializeExtension(WebRTCExtMainDir);

                if (theseOptions.Options.SpoofFontFingerprint)
                    await FontDefenderAddon.InitializeExtension(FontDefenderExtMainDir);

                //if (BrowserType == SystemBrowserType.Chrome)
                await NavigatorAddon.InitializeExtension(NavigatorExtMainDir, await BrowserDefaultLaunchSettings.Instance());
            }
        }
        //if (BrowserType == SystemBrowserType.Firefox)
        //    foreach (var dir in Extensions)
        //    {
        //        if(Directory.Exists(dir))
        //        {
        //            await IOtil.CreateZipAsync(Path.Combine(BrowserProfileAddonsDir, GettmpFname), dir);
        //            await IOtil.DeleteDExistsAsync(dir);
        //        }
        //    }

        if (HasProxyLogin)
        {
            await IOtil.CreateDirectory(ProxyExtMainDir);
            string startUrl = Starturl.Contains(ProxyAddonUtil.UrlSchemeEnd) ?
                Starturl : $"{ProxyAddonUtil.HTTPSScheme}{Starturl}";
            if (BrowserType == SystemBrowserType.Firefox)
            {

                //string loadUrl =
                //    startUrl.Contains(ProxyAddonUtil.DomainLevelDelimiter) ?
                //    @$", async () => {{ 
                //            let tabs = await browser.tabs.query({{}});
                //            if (tabs.length > 1) {{
                //                await browser.tabs.remove(tabs[tabs.length - 1].id);
                //            }}
                //            browser.tabs.update({{ url:""{startUrl}"" }}); 
                //      }});"
                //    : ");";

                await IOtil.CreateZipAsync(Path.Combine(ProxyExtMainDir, GettmpFname), new Dictionary<string, string>
                {
                    { "manifest.json", ProxyAddonUtil.GetManifest() },
                    { "background.js", ProxyAddonUtil.GetBgJs(startUrl, UserProfile.Proxy) }
                });
            }
            else
            {
                await IOtil.WriteTextToFileAsync(Path.Combine(ProxyExtMainDir, "manifest.json"), ProxyAddonUtil.GetManifestv3());
                await IOtil.WriteTextToFileAsync(
                    Path.Combine(ProxyExtMainDir, "background.js"),
                    ProxyAddonUtil.GetBgJsv3(startUrl, UserProfile.Proxy));
            }
        }
    }

    string GettmpFname => Guid.NewGuid().ToString() + ".zip";

    protected virtual async Task StartProcess()
    {
        // var tcs = new TaskCompletionSource<string>();

        Brocess = ProUtil.Createa(BrowserExeFilePath, GetCommandLineArguments());
        Brocess.Start();

        if (IsMao)
        {
            Handle = Brocess.Handle;
            Brocess.Exited += (s, e) => { Cleanup(); };
            int tryCount = 0;
            while(Brocess?.HasExited == false && 
                    MacOSUtil.FindWindowByPID(Brocess.Id) == null &&
                    tryCount++ < 36)
                await Task.Delay(1000);
            
            MacOSWindowListener.Instance.AddPid(Brocess.Id);

            MacOSWindowListener.Instance.WindowForegroundChanged += OnWindowForeground;
        }
        else
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            //Brocess.WaitForInputIdle();
            await Task.Delay(1800);

            if (BrowserType != SystemBrowserType.Firefox)
            {
                string windowHandle = null;
                while (IsRunning)
                {
                    windowHandle = await GetWebSocketDebuggerUrlAsync().ConfigureAwait(false);
                    if (windowHandle.HasAny())
                        break;

                    await Task.Delay(250);
                }
                if(!windowHandle.HasAny())
                {
                    Cleanup();
                    return;
                }

                await TaskUtil.AwaitFor(()=>Brocess?.MainWindowHandle != IntPtr.Zero, 18);
                Handle = Brocess?.MainWindowHandle ?? IntPtr.Zero;
                if (Brocess?.HasExited == false)
                    Brocess.Exited += (s, e) => 
                    { Cleanup(); };
            }
            else
            {
                TaskCompletionSource<Process?> thisTcs = new();
                new Thread(()=>
                {
                    for(int i = 0; i < 18; i++)
                    {
                        ExUtil.TryCatch(()=>
                        {
                            var currentProcesses = Process.GetProcessesByName("firefox");
                            foreach (var p in currentProcesses)
                            {
                                if (Brocess != null && p.ParentProcessId() == Brocess.Id)
                                {
                                    var childProcess = Process.GetProcessById(p.Id);
                                    if (childProcess?.HasExited == false)
                                    {
                                        IntPtr thishandle = U32til.FindMainWindowHandle(childProcess.Id);
                                        if(U32.IsWindow(thishandle))
                                        {
                                            thisTcs.TrySetResult(childProcess);
                                            break;
                                        }
                                    }
                                }
                            }
                        });
                        if(Handle != IntPtr.Zero)
                            break;
                        Thread.Sleep(100);
                    }
                    if (Handle == IntPtr.Zero)
                        thisTcs.TrySetResult(null);
                }).Start();
                Brocess = await thisTcs.Task;
                Handle = Brocess?.MainWindowHandle ?? IntPtr.Zero;
            }
            #pragma warning restore CA1416 // Validate platform compatibility

            SetWin32Events();
        }

        if (Brocess?.HasExited == false)
            OPtcs.TrySetResult(true);
        else
            Cleanup();
    }

    void OnWindowForeground(int i) 
    {
        if (i == Brocess.Id)
            eventAggregator.Pub<ForegroundUserSystemBrowserEvent>(GetArgs);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private void SetWin32Events()
    {
        if (Brocess?.HasExited == false && Handle != IntPtr.Zero)
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
                        .Publish(GetArgs);
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
                var r = await OPtcs.Task;

                if (!r || Handle == IntPtr.Zero || Brocess == null || Brocess.HasExited)
                    Cleanup();
                break;

            default:
                break;
        }
    }

    public void Cleanup()
    {
        if (IsMao)
        {
            MacOSWindowListener.Instance.WindowForegroundChanged -= OnWindowForeground;
            MacOSWindowListener.Instance.RemPid(Brocess.Id);
        }
        else
        {
#pragma warning disable CA1416 // Validate platform compatibility
            ExUtil.TryCatch(() =>
            {
                foreach (var item in winEventHooks)
                {
                    U32.UnhookWinEvent(item);
                }
            });
#pragma warning restore CA1416 // Validate platform compatibility
        }

        var r = OPtcs.TrySetResult(false);
        Brocess = null;
        Handle = IntPtr.Zero;
        OnProcessClosed?.Invoke(options);
    }
    

    protected async Task EnsureProfileFolderCreated()
    {
        await IOtil.CreateDirectory(BrowserProfileFolderPath);
    }

    protected virtual Task InitializeProfileFolder()
    {
        return Task.CompletedTask;
    }

    protected virtual List<string> GetClearCommandLineArgumentsList()
    {
        // "--in-process-gpu","--disable-software-rasterizer",
        List<string> args =
            [
                "--disable-session-crashed-bubble",
                "--hide-crash-restore-bubble",
                "--restore-last-session",
                "--profile-directory=Default",
                "--ash-no-nudges",
                "--disable-domain-reliability",
                "--no-default-browser-check",
                "--no-first-run",
                "--disable-field-trial-config",
                $"--remote-debugging-port={Port}",
                //$"--window-name=\"{UserProfile.Title}\"",
            ];

        if (UserProfile.Proxy?.CanUse == true && UserProfile.Proxy.Host.HasAny())
        {
            args.Add($"--proxy-server={UserProfile.Proxy.ServerForRequest}");
            //args.Add($"--proxy-server=http://{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}");
        }

       //if (!UserProfile.WebBrowser.WebRTC)
       //{
       //    args.Add("--disable-media-stream");
       //    args.Add("--disable-webrtc-hw-encoding");
       //    args.Add("--disable-webrtc-hw-decoding");
       //    args.Add("--webrtc-stun-probe-trial");
       //    args.Add("--use-fake-device-for-media-stream");
       //    args.Add("--enable-webrtc-hide-local-ips-with-mdns");
       //    args.Add("--force-webrtc-ip-handling-policy");
       //    args.Add("--enforce-webrtc-ip-permission-check");
       //}

        //if (!UserProfile.WebBrowser.WebGL)
        //{
            //args.Add("--disable-webgl");
        //}

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

        args.Add($"--user-data-dir=\"{BrowserProfileFolderPath}\"");

        return args;
    }

    protected virtual string GetCommandLineArguments()
    {
        List<string> args = GetCommandLineArgumentsList();
        
        if (GetLoadExtensionsArgument().Get() is string exts)
            args.Add($"--load-extension=\"{exts}\"");
       
        //if(!IsMao)
            args.Add($"{Starturl}");
        
        return string.Join(" ", args);
    }

    public virtual string GetLoadExtensionsArgument()
    {
        List<string> exts = [];
        foreach (var dir in Extensions)
        {
            if (Directory.Exists(dir))
                exts.Add(dir);
        }

        if (Directory.Exists(BrowserExtensionsFolderPath))
            exts.AddRange(Directory.GetDirectories(BrowserExtensionsFolderPath));

        return exts.ToCommaSeparatedString();
    }


    private async Task<string> GetWebSocketDebuggerUrlAsync()
    {
        string url = $"http://localhost:{Port}/json";
        using HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5) // Set a timeout of 5 seconds
        };

        try
        {
            string jsonResponse = await client.GetStringAsync(url);
            JArray targets = JArray.Parse(jsonResponse);

            foreach (JObject target in targets)
            {
                if (target["type"].ToString() == "page") // Assuming you want to debug a page
                {
                    return target["webSocketDebuggerUrl"].ToString();
                }
            }

            return null; // No suitable debugger URL found
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            // Handle timeout
            Console.WriteLine("The request timed out.");
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Handle other HTTP request exceptions
            Console.WriteLine($"HttpRequestException: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    private async Task<IntPtr> GetWindowHandleAsync()
    {
        string webSocketDebuggerUrl = await GetWebSocketDebuggerUrlAsync();
        using ClientWebSocket webSocket = new ClientWebSocket();
        Uri uri = new Uri(webSocketDebuggerUrl);
        await webSocket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine("Connected to WebSocket.");

        string command = "{\"id\": 1, \"method\": \"Browser.getWindowForTarget\"}";
        await SendCommandAsync(webSocket, command);

        string response = await ReceiveResponseAsync(webSocket);
        Console.WriteLine($"Received response: {response}");

        JObject jr = JObject.Parse(response);
        return (IntPtr)jr["result"]["windowId"].ToObject<int>();
    }

    private async Task<string> GetTargetIdAsync(ClientWebSocket webSocket)
    {
        string command = "{\"id\": 2, \"method\": \"Target.getTargets\"}";
        await SendCommandAsync(webSocket, command);

        string response = await ReceiveResponseAsync(webSocket);
        Console.WriteLine($"Received response: {response}");

        JObject jr = JObject.Parse(response);
        return jr["result"]["targetInfos"]
            .FirstOrDefault(t => t["type"].ToString() == "page")?["targetId"]
            .ToString();
    }

    private async Task AttachToTargetAsync(ClientWebSocket webSocket, string targetId)
    {
        string command = $"{{\"id\": 3, \"method\": \"Target.attachToTarget\", \"params\": {{\"targetId\": \"{targetId}\"}}}}";
        await SendCommandAsync(webSocket, command);

        string response = await ReceiveResponseAsync(webSocket);
        Console.WriteLine($"Received response: {response}");
    }

    private async Task<int> GetProcessIdAsync(ClientWebSocket webSocket, string targetId)
    {
        string command = $"{{\"id\": 4, \"method\": \"Target.sendMessageToTarget\", \"params\": {{\"targetId\": \"{targetId}\", \"message\": \"{{\\\"id\\\": 5, \\\"method\\\": \\\"Browser.getWindowForTarget\\\"}}\"}}}}";
        await SendCommandAsync(webSocket, command);

        string response = await ReceiveResponseAsync(webSocket);
        Console.WriteLine($"Received response: {response}");

        JObject jr = JObject.Parse(response);
        return jr["result"]["processId"].ToObject<int>();
    }

    private async Task SendCommandAsync(ClientWebSocket webSocket, string command)
    {
        byte[] bytesToSend = Encoding.UTF8.GetBytes(command);
        await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> ReceiveResponseAsync(ClientWebSocket webSocket)
    {
        StringBuilder responseBuilder = new StringBuilder();
        byte[] buffer = new byte[1024];
        WebSocketReceiveResult result;

        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            responseBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return responseBuilder.ToString();
    }
}