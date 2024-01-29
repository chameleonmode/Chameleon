using Chameleon.Interfaces.Entities;
using System;

namespace Chameleon.Interfaces.ProxyCredit
{
    public interface IProxyCreditOrder
         : IEntity<Guid>
    {
        decimal Amount { get; set; }
        string Url { get; set; }
    }
}
