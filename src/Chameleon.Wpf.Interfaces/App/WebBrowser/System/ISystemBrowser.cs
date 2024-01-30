using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowser : ISingletonDependency
    {
        void Open(ISystemBrowserLaunchOptions options);
    }
}
