using Chameleon.Interfaces.Ioc;
using Chameleon.lib.Common.Enums;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface ISystemBrowserManager : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        ISystemBrowser Get(SystemBrowserType browserType);
    }
}
