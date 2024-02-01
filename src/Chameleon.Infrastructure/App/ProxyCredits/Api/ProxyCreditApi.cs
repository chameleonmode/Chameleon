using Chameleon.App.Shared.Proxies;
using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.ProxyCredit.Api.Dto;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.ProxyCredit;

namespace Chameleon.Infrastructure.ProxyCredit.Api
{
    public class ProxyCreditApi
        : ApiLayer<ProxyCreditDto>
        , IProxyCreditApi
    {
        public ProxyCreditApi(
            IApiClient apiClient
            ) : base(apiClient, "ProxyCredit")
        {
        }

        public ProxyCreditDto BuyCredits(BuyCreditsDto input)
        {
            var dto = _apiClient.Post<ProxyCreditDto>(GetEndpointUrl("BuyCredits"), input);
            ThrowIfInvalidId(dto);
            return dto;
        }

        public ProxyCreditOrderDto CreateOrder(CreateBuyCreditOrderDto input)
        {
            var dto = _apiClient.Post<ProxyCreditOrderDto>(GetEndpointUrl("CreateOrder"), input);
            ThrowIfInvalidId(dto);
            return dto;
        }

        public ProxyCreditDto GetCredits()
        {
            var dto = _apiClient.Get<ProxyCreditDto>(GetEndpointUrl("GetCredits"));
            if (dto.Amount != 0)
            {
                ThrowIfInvalidId(dto);
            }
            return dto;
        }
    }
}
