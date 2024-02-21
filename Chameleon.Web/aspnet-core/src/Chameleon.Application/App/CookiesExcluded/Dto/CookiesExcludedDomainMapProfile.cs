using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class CookiesExcludedDomainMapProfile : AutoMapper.Profile
    {
        public CookiesExcludedDomainMapProfile()
        {
            CreateEntityDtoMap<CookiesExcludedDomainDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<CookiesExcludedDomainBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateCookiesExcludedDomainDto>();
            CreateEntityDtoMap<UpdateCookiesExcludedDomainDto>();
        }

        private IMappingExpression<TDto, CookiesExcludedDomain> CreateEntityDtoMap<TDto>()
            where TDto : CookiesExcludedDomainEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, CookiesExcludedDomain> CreateBaseDtoMap<TDto>()
            where TDto : CookiesExcludedDomainBaseDto
        {
            return CreateMap<TDto, CookiesExcludedDomain>()
                .ForMember(model => model.Domain, options => options.MapFrom(dto => dto.Domain))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
