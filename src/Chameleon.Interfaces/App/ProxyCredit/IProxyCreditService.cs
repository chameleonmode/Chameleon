using Chameleon.App.Shared.Proxies;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.ProxyCredit
{
    public interface IProxyCreditService
        : ISingletonDependency
    {
        IProxyCredit GetCredits();
        IProxyCredit BuyCredits(BuyCreditsDto input);
        IProxyCreditOrder CreateOrder(CreateBuyCreditOrderDto input);
    }
}
