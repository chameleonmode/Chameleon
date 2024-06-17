namespace Chameleon.SystemBrowser;
public class SystemBrowserManager(IHaveContainerProvider containerProvider) : ISystemBrowserManager
{
    private readonly Dictionary<SystemBrowserType, Type> _mapping =
        new Dictionary<SystemBrowserType, Type>()
        {
                { SystemBrowserType.Chrome, typeof(IChromeSystemBrowser) },
                { SystemBrowserType.Firefox, typeof(IFirefoxSystemBrowser) },
                { SystemBrowserType.Brave, typeof(IBraveSystemBrowser) },
        };

    public static IPlaywright Blaywright { get; set; }
    public ISystemBrowser Get(SystemBrowserType browserType)
    {
        if (_mapping.TryGetValue(browserType, out var type))
        {
            return (ISystemBrowser)containerProvider.Resolve(type);
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
