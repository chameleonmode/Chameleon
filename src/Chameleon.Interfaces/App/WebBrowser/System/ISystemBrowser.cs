using Chameleon.Interfaces.Ioc;
using System.Collections.Concurrent;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowser : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        ConcurrentDictionary<int, ISystemBrowserInstance> Instances { get; }
        Task<ISystemBrowserInstance> Open(ISystemBrowserLaunchOptions options);
    }
}
