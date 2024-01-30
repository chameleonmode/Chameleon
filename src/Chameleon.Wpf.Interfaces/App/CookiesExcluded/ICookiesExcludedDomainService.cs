using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public interface ICookiesExcludedDomainService
        : ISingletonDependency
    {
        ICookiesExcludedDomains GetAll(int profileId, bool ignoreCache = false);
        void Update(ICookiesExcludedDomain cookiesExcludedDomain);
        void Delete(ICookiesExcludedDomain cookiesExcludedDomain);
        ICookiesExcludedDomain Create(ICookiesExcludedDomain boocookiesExcludedDomainkmark);
    }
}
