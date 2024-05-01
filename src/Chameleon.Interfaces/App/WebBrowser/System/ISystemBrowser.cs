using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowser : ISingletonDependency
    {
        Dictionary<int, ISystemBrowserInstance> Instances { get; }
        Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions options);
    }
}
