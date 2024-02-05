using AutoMapper;
using Chameleon.Interfaces.CookiesExcluded;
using System.Collections.Generic;
using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Repository;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public class CookiesExcludedDomainService : ICookiesExcludedDomainService
    {
        private readonly IMapper _mapper;
        private readonly ICookiesExcludedDomainRepository _cookiesExcludedDomainRepository;
        private readonly Dictionary<int, CookiesExcludedDomains> _entities
                   = new Dictionary<int, CookiesExcludedDomains>();

        public CookiesExcludedDomainService(
          IMapper mapper,
          ICookiesExcludedDomainRepository cookiesExcludedDomainRepository)
        {
            _mapper = mapper;
            _cookiesExcludedDomainRepository = cookiesExcludedDomainRepository;
        }

        public void Delete(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            EnsureEntitiesContainsProfileId(cookiesExcludedDomain.ProfileId);

            _cookiesExcludedDomainRepository.Delete(cookiesExcludedDomain.Id);
            this.InvokeOnUiThread(() =>
            {
                _entities[cookiesExcludedDomain.ProfileId]
                .Remove(x => x.Id == cookiesExcludedDomain.Id);
            });
        }

        public ICookiesExcludedDomains GetAll(int profileId, bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                _entities.Clear();
            }

            if (_entities.TryGetValue(profileId, out var cookiesExcludedDomains))
            {
                return cookiesExcludedDomains;
            }

            var request = new UserProfileGetAllRequestDto(profileId);
            var items = _cookiesExcludedDomainRepository.GetAll(ignoreCache, request);
            cookiesExcludedDomains = new CookiesExcludedDomains(profileId, items);
            _entities[profileId] = cookiesExcludedDomains;

            return cookiesExcludedDomains;
        }

        public ICookiesExcludedDomain Create(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            EnsureEntitiesContainsProfileId(cookiesExcludedDomain.ProfileId);
            var cookiesExcludedDomainkMap = _mapper.Map<CookiesExcludedDomain>(cookiesExcludedDomain);
            _cookiesExcludedDomainRepository.Insert(cookiesExcludedDomainkMap);
            _entities[cookiesExcludedDomain.ProfileId].Add(cookiesExcludedDomain);

            return cookiesExcludedDomainkMap;
        }

        public void Update(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            var cookiesExcludedDomainMap = _mapper.Map<CookiesExcludedDomain>(cookiesExcludedDomain);
            this.InvokeOnUiThread(() => _cookiesExcludedDomainRepository.Update(cookiesExcludedDomainMap));
        }

        private void EnsureEntitiesContainsProfileId(int profileId)
        {
            if (!_entities.ContainsKey(profileId))
            {
                _entities[profileId] = new CookiesExcludedDomains(profileId, GetAll(profileId));
            }
        }
    }
}
