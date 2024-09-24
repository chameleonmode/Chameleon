using Chameleon.App.Shared.Proxies;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Proxies
{
    public interface IProxyService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IProxyCountry CurrentCountry { get; set; }
        IList<IProxyCountry> GetCountries();
        IList<IProxyAccess> GetAccess(ProxyAccessRequestDto input);
    }
}
