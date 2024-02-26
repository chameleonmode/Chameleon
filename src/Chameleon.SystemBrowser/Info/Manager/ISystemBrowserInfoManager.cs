using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.SystemBrowser
{
    public interface ISystemBrowserInfoManager : ISingletonDependency
    {
        IReadOnlyList<ISystemBrowserInfo> GetAll();
        ISystemBrowserInfo FindByName(string browserName);
    }
}
