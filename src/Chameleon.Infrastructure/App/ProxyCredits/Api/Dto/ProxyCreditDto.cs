using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.ProxyCredit.Api.Dto
{
    public class ProxyCreditDto
        : IEntityDto<int>
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
    }
}
