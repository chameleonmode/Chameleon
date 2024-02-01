using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.CookiesExcluded;

namespace Chameleon.Infrastructure.CookiesExcluded
{
    public class CookiesExcludedDomainMapProfile : Profile
    {
        public CookiesExcludedDomainMapProfile()
        {
            DtoMap();
            CreateDtoMap();
        }
        private void DtoMap()
        {
            var map = CreateMap<CookiesExcludedDomainDto, CookiesExcludedDomain>();
            map.ReverseMap();
        }

        private void CreateDtoMap()
        {
            CreateMap<ICookiesExcludedDomain, CreateCookiesExcludedDomainDto>();
        }
    }
}
