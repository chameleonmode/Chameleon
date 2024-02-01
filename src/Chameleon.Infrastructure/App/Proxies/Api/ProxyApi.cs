using Chameleon.App.Shared.Proxies;
using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Proxies.Api.Dto;
using Chameleon.Infrastructure.ProxyCredit.Api.Dto;
using Chameleon.Interfaces.Api;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Proxies.Api
{
    public class ProxyApi
        : ApiLayer<ProxyCreditDto>
        , IProxyApi
    {
        public ProxyApi(
            IApiClient apiClient
            ) : base(apiClient, "Proxy")
        {
        }

        public IList<ProxyAccessDto> GetAccess(ProxyAccessRequestDto input)
        {
            return _apiClient.Get<ProxyAccessDto[]>(GetEndpointUrl("GetAccess"), input);
        }

        public IList<ProxyCountryDto> GetCountries()
        {
            return _apiClient.Get<ProxyCountryDto[]>(GetEndpointUrl("GetCountries"));
        }
    }
}
