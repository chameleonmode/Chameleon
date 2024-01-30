using Chameleon.App.Shared.Proxies;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Proxies
{
    public interface IProxyService
        : ISingletonDependency
    {
        IProxyCountry CurrentCountry { get; set; }
        IList<IProxyCountry> GetCountries();
        IList<IProxyAccess> GetAccess(ProxyAccessRequestDto input);
    }
}
