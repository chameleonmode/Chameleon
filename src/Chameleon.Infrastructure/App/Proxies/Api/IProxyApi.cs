using Chameleon.App.Shared.Proxies;
using Chameleon.Infrastructure.Proxies.Api.Dto;
using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Proxies.Api
{
    public interface IProxyApi
        : ISingletonDependency
    {
        IList<ProxyCountryDto> GetCountries();
        IList<ProxyAccessDto> GetAccess(ProxyAccessRequestDto input);
    }
}
