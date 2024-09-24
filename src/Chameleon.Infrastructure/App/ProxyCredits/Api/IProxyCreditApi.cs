using Chameleon.App.Shared.Proxies;
using Chameleon.Infrastructure.ProxyCredit.Api.Dto;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.ProxyCredit;

namespace Chameleon.Infrastructure.ProxyCredit.Api
{
    public interface IProxyCreditApi
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        ProxyCreditDto GetCredits();
        ProxyCreditDto BuyCredits(BuyCreditsDto input);
        ProxyCreditOrderDto CreateOrder(CreateBuyCreditOrderDto input);
    }
}
