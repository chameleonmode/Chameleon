using Chameleon.App.Shared.Proxies;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.ProxyCredit
{
    public interface IProxyCreditService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IProxyCredit GetCredits();
        IProxyCredit BuyCredits(BuyCreditsDto input);
        IProxyCreditOrder CreateOrder(CreateBuyCreditOrderDto input);
    }
}
