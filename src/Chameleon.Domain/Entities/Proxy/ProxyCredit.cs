using Chameleon.Interfaces.ProxyCredit;

namespace Chameleon.Domain.Entities.Proxy
{
    public class ProxyCredit : IProxyCredit
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
    }
}
