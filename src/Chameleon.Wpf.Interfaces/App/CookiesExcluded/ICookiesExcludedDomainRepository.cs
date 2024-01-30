using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.CookiesExcluded
{
    public interface ICookiesExcludedDomainRepository
        : IRepository<ICookiesExcludedDomain, int, UserProfileGetAllRequestDto>
    {
    }
}
