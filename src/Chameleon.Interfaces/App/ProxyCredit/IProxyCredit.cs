using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.ProxyCredit
{
    public interface IProxyCredit
        : IEntity<int>
    {
        decimal Amount { get; set; }
    }
}
