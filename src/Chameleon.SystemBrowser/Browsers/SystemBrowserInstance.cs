using Chameleon.Core.Extensions;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Proxy;
using Chameleon.Prism.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading.Tasks;
using Chameleon.SystemBrowser.Automation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Chameleon.Common.WinApiBridge;
using System.Reflection.Metadata;
using Microsoft.Playwright;
using System.Security;
using Newtonsoft.Json.Linq;
using Chameleon.Interfaces.Settings;
using System.Reflection;

namespace Chameleon.SystemBrowser.Common
{
    public abstract class SystemBrowserInstance : ISystemBrowserInstance
    {
        public event Action<ISystemBrowserLaunchOptions> OnProcessClosed;

        protected readonly string _browserExeFilePath;
        protected readonly string _browserDataFolderPath;
        protected readonly string _browserProfileFolderPath;

        private readonly IEventAggregator _eventAggregator;
        private readonly ISystemBrowserLaunchOptions _options;
        private readonly IUserDefaultSettingsService _userDefaultsSettingsService;
                                                                   
        private readonly List<IntPtr> winEventHooks = new List<IntPtr>(); 
        private User32.WinEventDelegate winEventsCaptureDelegate;

        public ISystemBrowserLaunchOptions Options => _options;
        public IUserProfile UserProfile => _options.UserProfile;

        private string proxyextdir;


        public string Starturl { get; private set; } = "https://www.duckduckgo.com/";

        protected string BrowserExtensionsRootFolderPath { get; set; }
        protected string BrowserExtensionsFolderPath { get; set; }

        public Process? Brocess { get; private set; } = null;
        public IntPtr Handle { get; private set; } = IntPtr.Zero;

        public IBrowserContext BrowserContext { get; set; }

        public int Port { get; private set; }

        protected SystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            IUserDefaultSettingsService userDefaultsSettingsService,
            string browserDataFolderPath,
            string browserExeFilePath
            )
        {
            _eventAggregator = eventAggregator;
            _options = options;
            _userDefaultsSettingsService = userDefaultsSettingsService;
            _browserExeFilePath = browserExeFilePath;
            _browserDataFolderPath = browserDataFolderPath;
            _browserProfileFolderPath = Path.Combine(_browserDataFolderPath, BrowserType.ToString(), UserProfile.Id.ToString());

            if (OperatingSystem.IsMacOS())
                BrowserExtensionsRootFolderPath = "/Applications/Chameleon.app/Contents/Resources/BrowserExtensions/mac";
            else
                BrowserExtensionsRootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "BrowserExtensions");
        }

        public async Task Open()
        {
            if (Brocess is null || Handle == IntPtr.Zero || !User32.IsWindow(Handle))
            {
                var userSettings = await Task.Run(() => _userDefaultsSettingsService.GetAll());
                if (userSettings != null && userSettings.Any())
                {
                    Starturl = userSettings[new Random().Next(userSettings.Count)].DefaultUrl;
                }
                Port = SystemBrowserInstance.NextFreePort(9613);
                await EnsureProfileFolderCreated();
                await InitializeProfileFolder();  
                //if (BrowserType != SystemBrowserType.Firefox)
                //{                         
                    await InitializeExtensionPath();
                    await StartProcess();
                //}
                //else
                //{
                //    await BrowserContext.Pages[0].BringToFrontAsync();
                //    Handle = User32.GetForegroundWindow();
                //    var iid = User32.GetWindowThreadProcessId((IntPtr)Handle, out uint id);
                //    Brocess = Process.GetProcessById((int)id);
                //    SetWin32Events();
                //    PublishOpendedEvent(Brocess);
                //}
            }

            if (Handle != IntPtr.Zero)
            {
                User32.SetForegroundWindow((IntPtr)Handle);
                User32.SetActiveWindow((IntPtr)Handle);
            }
        }

        protected virtual async Task InitializeExtensionPath()
        {
            //proxyext
            proxyextdir = Path.Combine(_browserProfileFolderPath, "ChameleonAutoExt");
            await Task.Run(() => 
            {
               var old_proxyextdir = Path.Combine(_browserProfileFolderPath, "proxyext");
                if (Directory.Exists(old_proxyextdir))
                    Directory.Delete(old_proxyextdir, true);

                if (Directory.Exists(proxyextdir))
                    Directory.Delete(proxyextdir, true);
            });

            proxyextdir = Path.Combine(_browserProfileFolderPath, "ChameleonAutoExt", Guid.NewGuid().ToString());

            if (HasProxyLogin)
            {
                //from：https://github.com/henices/Chrome-proxy-helper
                var manifest_json = """
                {
                  "manifest_version": 3,
                  "name": "Chameleon Auto Proxy",
                  "version": "1.0.0",
                  "permissions": [
                    "webRequest",
                    "webRequestBlocking",
                    "webRequestAuthProvider",
                    "<all_urls>"
                  ],
                  "host_permissions": [
                    "<all_urls>"
                  ],
                  "background": {
                    "service_worker": "background.js"
                  }
                }
                """;
                //"background": {
                //    "service_worker": "background.js"
                //  }
                //"background": {
                //  "scripts": ["background.js"]
                //}
                //var background_js = """
                //          chrome.webRequest.onAuthRequired.addListener((details, callback) => {
                //              callback({
                //                authCredentials: {
                //          """
                //            + "username:" + $"\"{UserProfile.Proxy.UserName}\","
                //            + "password: " + $"\"{UserProfile.Proxy.Password}\"" +
                //           """
                //          }
                //        });
                //      },
                //      { urls: ['<all_urls>'] },
                //      ['asyncBlocking']
                //    );
                //    """;
                var background_js = """
                          chrome.webRequest.onAuthRequired.addListener((details) => {
                          return {
                          authCredentials: {
                          """
                            + "username:" + $"\"{UserProfile.Proxy.UserName}\","
                            + "password: " + $"\"{UserProfile.Proxy.Password}\"" +
                           """
                          }
                        };
                      },
                      { urls: ['<all_urls>'] },
                      ['blocking']
                    );
                    """;

                if (!Directory.Exists(proxyextdir))
                    Directory.CreateDirectory(proxyextdir);

                await File.WriteAllTextAsync(Path.Combine(proxyextdir, "manifest.json"), manifest_json);
                await File.WriteAllTextAsync(Path.Combine(proxyextdir, "background.js"), background_js);
            }

            BrowserExtensionsFolderPath = Path.Combine(BrowserExtensionsRootFolderPath, BrowserType.ToString());
        }
        protected virtual async Task StartProcess()
        {
            Brocess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _browserExeFilePath,
                    Arguments = GetCommandLineArguments(),
                    UseShellExecute = true,
                    ErrorDialog = true,
                },
                EnableRaisingEvents = true,
            };
            Brocess.Start();
            await Task.Delay(500);
            Brocess.Refresh();

            if (BrowserType == SystemBrowserType.Firefox)
            {
                await GotMainFFHandle(Brocess, 0);
            } 
            else
                Brocess.Exited += new EventHandler(Process_Exited);

            //if (BrowserType == SystemBrowserType.Firefox)
            //    await Brocess.WaitForExitAsync();

            int waited = 0;
            do
            {
                if (User32.IsWindow(Handle))
                    break;

                Handle = Brocess.MainWindowHandle;
                await Task.Delay(500);
            }
            while (waited++ <= 9 && Handle == IntPtr.Zero);

            SetWin32Events();
            PublishOpendedEvent(Brocess);
        }

        private void SetWin32Events()
        {
            if (Brocess != null && Handle != null)
            {
                winEventsCaptureDelegate = WinEventProc;
                // capture EVENT_OBJECT_FOCUS
                this.winEventHooks.Add(User32.SetWinEventHook(
                    User32Events.EVENT_OBJECT_FOCUS,
                    User32Events.EVENT_OBJECT_FOCUS,
                    IntPtr.Zero,
                    winEventsCaptureDelegate,
                    (uint)Brocess.Id,
                    0,
                    (uint)User32Events.WINEVENT_OUTOFCONTEXT));

                if (BrowserType == SystemBrowserType.Firefox)
                {
                    // capture window close
                    this.winEventHooks.Add(User32.SetWinEventHook(
                        User32Events.EVENT_OBJECT_DESTROY,
                        User32Events.EVENT_OBJECT_DESTROY,
                        IntPtr.Zero,
                        winEventsCaptureDelegate,
                        (uint)Brocess.Id,
                        0,
                        (uint)User32Events.WINEVENT_OUTOFCONTEXT));
                }
                else
                {
                }

                //TODO: User32.SendMessage((IntPtr)Handle, User32.WM_SETTEXT,0, new System.Text.StringBuilder(UserProfile.Title));

                User32.SetForegroundWindow((IntPtr)Handle);
                User32.SetActiveWindow((IntPtr)Handle);

                _eventAggregator
                         .GetEvent<ForegroundUserSystemBrowserEvent>()
                         .Publish(GetArgs(Brocess));
            }
        }
        private void WinEventProc(IntPtr hWinEventHook, User32Events eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case User32Events.EVENT_OBJECT_FOCUS:
                    if(hwnd == Handle)
                    {
                        _eventAggregator
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
                    //normalSessions.Add(curDisplayKey);
                    break;

                case User32Events.EVENT_OBJECT_DESTROY:
                    if (hwnd == Handle || Brocess.HasExited)
                    Cleanup();
                    break;

                default:
                    break;
                    //return;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            //if (BrowserType == SystemBrowserType.Firefox)
            //{
            //    if (await GotMainFFHandle(sender as Process, 0))
            //        return;
            //}

            Cleanup();
        }

        private async Task<bool> GotMainFFHandle(Process? process, int trys)
        {
            Process[] firefoxInstances = Process.GetProcessesByName("firefox");

            foreach (Process firefoxInstance in firefoxInstances)
            {
                if (!firefoxInstance.HasExited &&
                    firefoxInstance.StartTime > process.StartTime &&
                    firefoxInstance.MainWindowHandle != IntPtr.Zero)
                {
                    Brocess = firefoxInstance;
                    Handle = Brocess.MainWindowHandle;
                    Brocess.Exited += new EventHandler(Process_Exited);
                    return true;
                }
            }
            if (trys++ < 5)
            {
                await Task.Delay(1000);
                return await GotMainFFHandle(process, trys);
            }
            else
                return false;
        }

        protected virtual async void Cleanup()
        {
            //_dynamicProxyServer?.Stop();
            foreach (var item in winEventHooks)
            {
                User32.UnhookWinEvent(item);
            }
            //if(BrowserContext != null)
            //    await BrowserContext.DisposeAsync();

            Brocess = null;
            Handle = IntPtr.Zero;
            OnProcessClosed?.Invoke(Options);
            _eventAggregator
               .GetEvent<ClosedUserSystemBrowserEvent>()
               .Publish(GetArgs(Brocess));
        }

        UserProfileSystemBrowserProcessEventArgs GetArgs(Process process) => new UserProfileSystemBrowserProcessEventArgs(
                    UserProfile,
                    BrowserType,
                    process,
                    Options.Url,
                    Options.SignIn
                    );
        private void PublishOpendedEvent(Process process)
        {
            _eventAggregator
                .GetEvent<OpenedUserSystemBrowserEvent>()
                .Publish(GetArgs(process));
        }

        private Task EnsureProfileFolderCreated()
        {
            if (!Directory.Exists(_browserProfileFolderPath))
            {
                Directory.CreateDirectory(_browserProfileFolderPath);
               
            }

            return OnProfileFolderCreated();
        }

        protected virtual Task OnProfileFolderCreated()
        {
            return Task.CompletedTask;
        }

        protected virtual Task InitializeProfileFolder()
        {
            return Task.CompletedTask;
        }

        protected virtual string GetCommandLineArguments()
        {
            var exts = GetLoadExtensionsArgument();
            List<string> args =
                [
                    $"--user-data-dir=\"{_browserProfileFolderPath}\"",
                    "--restore-last-session",
                    "--new-window",
                    $"--window-name=\"{UserProfile.Title}\"",
                    "--profile-directory=Default",
                    "--ash-no-nudges",
                    "--disable-domain-reliability",
                    "--in-process-gpu",
                    "--no-default-browser-check",
                    "--no-first-run",
                    "--disable-field-trial-config",
                    "--disable-software-rasterizer",
                    //"--disable-blink-features=\"BlockCredentialedSubresources\"",
                    $"--remote-debugging-port={Port}",
                    Starturl
                ];

            if (UserProfile.Proxy?.CanUse == true && UserProfile.Proxy.Host.HasAny())
            {
                args.Add($"--proxy-server=http://{UserProfile.Proxy.Host}:{UserProfile.Proxy.Port}");
                //args.Add($"--proxy-auth={UserProfile.Proxy.UserName}:{UserProfile.Proxy.Password}");

                if (Directory.Exists(proxyextdir))
                    exts = exts.HasAny() ? $"{exts},{proxyextdir}" : proxyextdir;
            }
            if (exts.HasAny())
                args.Add($"--load-extension=\"{exts}\"");

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

            return string.Join(" ", args);
        }
        public virtual string GetLoadExtensionsArgument()
        {
            if (!Directory.Exists(BrowserExtensionsFolderPath))
                return "";

            return Directory
                 .GetDirectories(BrowserExtensionsFolderPath)
                 .ToCommaSeparatedString();
        }

        public static bool IsFree(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] listeners = properties.GetActiveTcpListeners();
            int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
            return openPorts.All(openPort => openPort != port);
        }

        public static int NextFreePort(int port = 0)
        {
            port = (port > 0) ? port : new Random().Next(1, 65535);
            while (!IsFree(port))
            {
                port += 1;
            }
            return port;
        }  
        protected abstract SystemBrowserType BrowserType { get; }
        public bool HasProxyLogin => UserProfile.Proxy?.CanUse == true &&
            UserProfile.Proxy.Host.HasAny() &&
            UserProfile.Proxy.UserName.HasAny() &&
            UserProfile.Proxy.Password.HasAny();
    }
}
