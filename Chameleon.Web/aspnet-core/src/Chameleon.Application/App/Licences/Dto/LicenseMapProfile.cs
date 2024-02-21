using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class LicenseMapProfile : AutoMapper.Profile
    {
        public LicenseMapProfile()
        {
            CreateEntityDtoMap<LicenseDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<LicenseBaseDto>()
                .ReverseMap();
        }

        private IMappingExpression<TDto, License> CreateEntityDtoMap<TDto>()
            where TDto : LicenseEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, License> CreateBaseDtoMap<TDto>()
            where TDto : LicenseBaseDto
        {
            return CreateMap<TDto, License>()
                .ForMember(model => model.LicenseKey, options => options.MapFrom(dto => dto.LicenseKey))
                ;
        }
    }
}
