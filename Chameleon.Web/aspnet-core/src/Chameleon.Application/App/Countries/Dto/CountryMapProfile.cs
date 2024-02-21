using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class CountryMapProfile : AutoMapper.Profile
    {
        public CountryMapProfile()
        {
            CreateEntityDtoMap<CountryDto>()
                .ReverseMap()
                ;
        }

        private IMappingExpression<TDto, Country> CreateEntityDtoMap<TDto>()
            where TDto : CountryEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Country> CreateBaseDtoMap<TDto>()
            where TDto : CountryBaseDto
        {
            return CreateMap<TDto, Country>()
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.IsMetric, options => options.MapFrom(dto => dto.IsMetric))
                .ForMember(model => model.ISOCode2, options => options.MapFrom(dto => dto.ISOCode2))
                .ForMember(model => model.ISOCode3, options => options.MapFrom(dto => dto.ISOCode3))
                ;
        }
    }
}
