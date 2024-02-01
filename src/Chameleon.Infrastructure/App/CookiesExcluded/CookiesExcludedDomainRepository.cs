using AutoMapper;
using Chameleon.Common.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Profiles;
using Chameleon.Interfaces.CookiesExcluded;
using Chameleon.Interfaces.UserProfiles;
using Prism.Events;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public class CookiesExcludedDomainRepository
        : UserProfileItemRepository<
            CookiesExcludedDomain,
            ICookiesExcludedDomain,
            CookiesExcludedDomainDto,
            CreateCookiesExcludedDomainDto,
            CookiesExcludedDomainDto>
        , ICookiesExcludedDomainRepository
    {
        public CookiesExcludedDomainRepository(
            IMapper mapper,
            ICookiesExcludedDomainApi apiClient,
            IEventAggregator eventAggregator,
            IUserProfileRepository profileRepository
            ) : base(mapper, apiClient, eventAggregator, profileRepository)
        {
        }
    }
}
