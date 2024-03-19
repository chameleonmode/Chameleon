using Chameleon.Core.Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Chameleon.SystemBrowser
{
    public class SystemBrowserInfoManager : ISystemBrowserInfoManager
    {
        private string[] IgnoreStartMenuInternetKeys = new[]
        {
            "VMWAREHOSTOPEN.EXE"
        };

        private bool IsValidBrowserKeyName(string startMenuinternetKey)
        {
            if (IgnoreStartMenuInternetKeys.Any(
                key => key.Equals(startMenuinternetKey, StringComparison.InvariantCultureIgnoreCase))
                )
            {
                return false;
            }
            return true;
        }

        private List<SystemBrowserInfo> _installedBrowsers;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:Chameleon.SystemBrowser.SystemBrowserInfoManager.GetAll~System.Collections.Generic.IReadOnlyList{Chameleon.SystemBrowser.ISystemBrowserInfo}")]
        public IReadOnlyList<ISystemBrowserInfo> GetAll()
        {
            if (_installedBrowsers != null)
            {
                return _installedBrowsers.ToArray();
            }

            RegistryKey browserKeys;
            //on 64bit the browsers are in a different location
            browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            if (browserKeys == null)
            {
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            }

            _installedBrowsers = new List<SystemBrowserInfo>();

            var browserNames = browserKeys
                .GetSubKeyNames()
                .Where(IsValidBrowserKeyName)
                .ToList();

            for (var i = 0; i < browserNames.Count; ++i)
            {
                var browserName = browserNames[i];
                var browserKey = browserKeys.OpenSubKey(browserName);
                if (TryGetSystemBrowserInfo(browserKey, out var browserInfo))
                {
                    _installedBrowsers.Add(browserInfo);
                }
            }
            return _installedBrowsers.ToArray();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private bool TryGetSystemBrowserInfo(RegistryKey browserKey, out SystemBrowserInfo browser)
        {
            var browserKeyPath = (string)browserKey
                .OpenSubKey(@"shell\open\command")
                .GetValue(null);
            var browserPath = GetBrowserPath(browserKeyPath);
            if (browserPath != null && !File.Exists(browserPath))
            {
                browser = null;
                return false;
            }

            browser = new SystemBrowserInfo();
            browser.Name = (string)browserKey.GetValue(null);
            browser.Path = browserPath;

            var browserIconPath = browserKey.OpenSubKey(@"DefaultIcon");
            browser.IconPath = browserIconPath
                .GetValue(null)
                .ToString()
                .StripQuotes();

            if (browser.Path != null)
            {
                browser.Version = FileVersionInfo
                    .GetVersionInfo(browser.Path)
                    .FileVersion;
            }
            else
            {
                browser.Version = "unknown";
            }
            return true;
        }

        private string GetBrowserPath(string browserPath)
        {
            browserPath = browserPath.TrimStart('\"');
            var extensionIndex = browserPath.LastIndexOf(".exe");
            return browserPath.Substring(0, extensionIndex + 4);
        }

        public ISystemBrowserInfo FindByName(string browserName)
        {
            var browserInfo = GetAll()
                .Where(info => info.Name.IndexOf(browserName, StringComparison.InvariantCultureIgnoreCase) != -1)
                .FirstOrDefault();
            if (browserInfo == null)
            {
                throw new NotSupportedException(browserName);
            }
            return browserInfo;
        }
    }
}
