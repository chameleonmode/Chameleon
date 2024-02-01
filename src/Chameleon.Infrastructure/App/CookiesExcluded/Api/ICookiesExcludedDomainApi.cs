using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public interface ICookiesExcludedDomainApi
        : IApiLayer<CookiesExcludedDomainDto, int, CreateCookiesExcludedDomainDto, CookiesExcludedDomainDto>
        , ISingletonDependency
    {
    }
}
