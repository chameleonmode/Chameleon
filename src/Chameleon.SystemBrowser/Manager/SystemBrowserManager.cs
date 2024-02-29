using Chameleon.Core.Services;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Browsers.Brave;
using Chameleon.SystemBrowser.Chrome;
using Chameleon.SystemBrowser.Firefox;
using System;
using System.Collections.Generic;

namespace Chameleon.SystemBrowser
{
    public class SystemBrowserManager : ISystemBrowserManager
    {
        private readonly Dictionary<SystemBrowserType, Type> _mapping =
            new Dictionary<SystemBrowserType, Type>()
            {
                { SystemBrowserType.Chrome, typeof(IChromeSystemBrowser) },
                { SystemBrowserType.Firefox, typeof(IFirefoxSystemBrowser) },
                { SystemBrowserType.Brave, typeof(IBraveSystemBrowser) },
            };


        public ISystemBrowser Get(SystemBrowserType browserType)
        {
            if (_mapping.TryGetValue(browserType, out var type))
            {
                return (ISystemBrowser)ContainerProviderServiceLocator.Current.ContainerProvider.Resolve(type);
            }
            throw new KeyNotFoundException(browserType.ToString());
        }
    }
}
