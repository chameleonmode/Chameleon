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

namespace Chameleon.SystemBrowser.Common
{
    public abstract class SystemBrowserInstance
    {
        protected readonly string _browserExeFilePath;
        protected readonly string _browserDataFolderPath;
        protected readonly string _browserProfileFolderPath;
        protected string _browserExtensionsFolderPath;
        protected string _browserExtensionsRootFolderPath;
        protected DynamicProxyServer _dynamicProxyServer;

        private readonly IEventAggregator _eventAggregator;
        private readonly ISystemBrowserLaunchOptions _options;

        public ISystemBrowserLaunchOptions Options
            => _options;

        public IUserProfile UserProfile
            => _options.UserProfile;

        protected SystemBrowserInstance(
            IEventAggregator eventAggregator,
            ISystemBrowserLaunchOptions options,
            string browserDataFolderPath,
            string browserExeFilePath
            )
        {
            _eventAggregator = eventAggregator;
            _options = options;
            _browserExeFilePath = browserExeFilePath;
            _browserDataFolderPath = browserDataFolderPath;

            if (OperatingSystem.IsMacOS())
                _browserExtensionsRootFolderPath = "/Applications/Chameleon.app/Contents/Resources/BrowserExtensions/mac";
                //_browserExtensionsRootFolderPath = Path.Combine(
               //     "Applications",
              //      "Chameleon.app",
              //      "Contents", 
              //      "Resources",
              //      "BrowserExtensions", 
              //      "mac");
            else
                _browserExtensionsRootFolderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "BrowserExtensions");

            _browserProfileFolderPath = Path.Combine(_browserDataFolderPath,
                BrowserType.ToString(),
                UserProfile.Id.ToString()
                );
        }
        protected abstract SystemBrowserType BrowserType { get; }

        public void Open()
        {
            EnsureProfileFolderCreated();
            InitializeProfileFolder();
            InitializeExtensionPath();
            StartProcess();
        }

        protected virtual void InitializeExtensionPath()
        {
            _browserExtensionsFolderPath = GetExtensionPath(BrowserType);
        }

        protected void EnsureExtensionsFolderExistsAsCopyFrom(SystemBrowserType copyFromBrowserType)
        {
            if (!Directory.Exists(_browserExtensionsFolderPath))
            {
                var chromeExtensionsFolderPath = GetExtensionPath(copyFromBrowserType);
                var chromeExtensionsFolderInfo = new DirectoryInfo(chromeExtensionsFolderPath);
                chromeExtensionsFolderInfo.CopyTo(_browserExtensionsFolderPath);
            }
        }

        protected string GetExtensionPath(SystemBrowserType systemBrowserType)
        {
            if (systemBrowserType == SystemBrowserType.Brave)
                systemBrowserType = SystemBrowserType.Chrome;

            return Path.Combine(
                _browserExtensionsRootFolderPath,
                systemBrowserType.ToString()
                );
        }

        protected virtual async void StartProcess()
        {
            if (BrowserType == SystemBrowserType.Firefox)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _browserExeFilePath,
                        Arguments = GetCommandLineArguments(),
                        UseShellExecute = true,
                        ErrorDialog = true
                    },
                    EnableRaisingEvents = true
                };

                process.Exited += new EventHandler(Process_Exited);

                process.Start();

                new Thread(process.WaitForExit).Start();

                PublishOpendedEvent(process);
            }
            else
            {
                try
                {
                    await Play.Instance.LaunchPersistentContextAsync(
                         _browserProfileFolderPath,
                         _browserExeFilePath,
                         UserProfile,
                         Options.Url,
                         GetLoadExtensionsArgument());
                }
                catch(Exception ex) 
                {

                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if(BrowserType == SystemBrowserType.Firefox)
            {
                Process[] firefoxInstances = Process.GetProcessesByName("firefox");

                foreach (Process firefoxInstance in firefoxInstances)
                {
                    if (!firefoxInstance.HasExited)
                    {
                        return;
                    }
                }
            }
     
            Cleanup();
        }
        protected virtual void Cleanup()
        {
            //_dynamicProxyServer?.Stop();
        }

        private void PublishOpendedEvent(Process process)
        {
            var args = new UserProfileSystemBrowserProcessEventArgs(
                    UserProfile,
                    BrowserType,
                    process,
                    Options.Url,
                    Options.SignIn
                    );

            _eventAggregator
                .GetEvent<OpenedUserSystemBrowserEvent>()
                .Publish(args);
        }

        private void EnsureProfileFolderCreated()
        {
            if (Directory.Exists(_browserProfileFolderPath))
            {
                return;
            }

            Directory.CreateDirectory(_browserProfileFolderPath);
            OnProfileFolderCreated();
        }

        protected virtual void OnProfileFolderCreated()
        {
        }

        protected virtual void InitializeProfileFolder()
        {
        }

        protected virtual string GetCommandLineArguments()
        {
            return string.Empty;
        }
        public virtual string GetLoadExtensionsArgument()
        {
            return string.Empty;
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
    }
}
