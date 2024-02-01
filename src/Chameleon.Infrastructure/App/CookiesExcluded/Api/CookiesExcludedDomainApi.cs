using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public class CookiesExcludedDomainApi
        : ApiLayer<CookiesExcludedDomainDto, int, CreateCookiesExcludedDomainDto, CookiesExcludedDomainDto>
        , ICookiesExcludedDomainApi
    {
        public CookiesExcludedDomainApi(IApiClient apiClient)
            : base(apiClient, "CookiesExcludedDomain")
        { }
    }
}
