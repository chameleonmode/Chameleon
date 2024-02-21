using Abp.Application.Services;
using Chameleon.App.Dto;
using Chameleon.App.Shared.Proxies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IProxyAppService
        : IApplicationService
    {
        IList<ProxyCountryDto> GetCountries();
        IList<ProxyAccessDto> GetAccess(ProxyAccessRequestDto input);
    }
}
