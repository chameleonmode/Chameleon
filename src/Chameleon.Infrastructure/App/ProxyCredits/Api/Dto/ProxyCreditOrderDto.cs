using Chameleon.Infrastructure.Dto;
using Chameleon.Interfaces.ProxyCredit;
using System;

namespace Chameleon.Infrastructure.ProxyCredit.Api.Dto
{
    public class ProxyCreditOrderDto
        : IProxyCreditOrder
        , IEntityDto<Guid>
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Url { get; set; }
    }
}
