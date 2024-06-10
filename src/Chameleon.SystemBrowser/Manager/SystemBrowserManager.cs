using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;
using Chameleon.SystemBrowser.Browsers.Brave;
using Chameleon.SystemBrowser.Chrome;
using Chameleon.SystemBrowser.Firefox;

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

        private readonly IHaveContainerProvider _containerProvider;

        public SystemBrowserManager(IHaveContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public ISystemBrowser Get(SystemBrowserType browserType)
        {
            if (_mapping.TryGetValue(browserType, out var type))
            {
                return (ISystemBrowser)_containerProvider.Resolve(type);
            }
            throw new KeyNotFoundException(browserType.ToString());
        }

        public static ContainerServiceHelper Current
        {
            get
            {
                return (ContainerServiceHelper)ContainerServiceHelper.Current.ContainerProvider.Resolve<ISystemBrowserManager>();
            }
        }
    }
}
