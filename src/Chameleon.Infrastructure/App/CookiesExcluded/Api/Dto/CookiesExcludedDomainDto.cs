using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public class CookiesExcludedDomainDto
        : CreateCookiesExcludedDomainDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
