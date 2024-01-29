using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowserManager : ISingletonDependency
    {
        ISystemBrowser Get(SystemBrowserType browserType);
    }
}
