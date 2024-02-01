using Chameleon.App.Shared.Proxies;
using Chameleon.Infrastructure.ProxyCredit.Api;
using Chameleon.Infrastructure.ProxyCredit.Api.Dto;
using Chameleon.Interfaces.ProxyCredit;

namespace Chameleon.Infrastructure.ProxyCredit
{
    public class ProxyCreditService
        : IProxyCreditService
    {
        private readonly IProxyCreditApi _proxyCreditApi;

        public ProxyCreditService(
            IProxyCreditApi proxyCreditApi
            )
        {
            _proxyCreditApi = proxyCreditApi;
        }

        public IProxyCredit GetCredits()
        {
            var entityDto = _proxyCreditApi.GetCredits();
            return Map(entityDto);
        }

        public IProxyCredit BuyCredits(BuyCreditsDto input)
        {
            var entityDto = _proxyCreditApi.BuyCredits(input);
            return Map(entityDto);
        }

        public IProxyCreditOrder CreateOrder(CreateBuyCreditOrderDto input)
        {
            var entityDto = _proxyCreditApi.CreateOrder(input);
            return entityDto;
        }

        private IProxyCredit Map(ProxyCreditDto dto)
        {
            return new Domain.Entities.Proxy.ProxyCredit
            {
                Id = dto.Id,
                Amount = dto.Amount
            };
        }
    }
}
