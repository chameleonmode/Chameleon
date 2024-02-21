using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class BusinessMapProfile : AutoMapper.Profile
    {
        public BusinessMapProfile()
        {
            CreateEntityDtoMap<BusinessDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<BusinessBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateBusinessDto>();
            CreateEntityDtoMap<UpdateBusinessDto>();
        }

        private IMappingExpression<TDto, Business> CreateEntityDtoMap<TDto>()
            where TDto : BusinessEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, Business> CreateBaseDtoMap<TDto>()
            where TDto : BusinessBaseDto
        {
            return CreateMap<TDto, Business>()
                .ForMember(model => model.Title, options => options.MapFrom(dto => dto.Title))
                .ForMember(model => model.CompanyName, options => options.MapFrom(dto => dto.CompanyName))
                .ForMember(model => model.Department, options => options.MapFrom(dto => dto.Department))
                .ForMember(model => model.PhoneNumber, options => options.MapFrom(dto => dto.PhoneNumber))
                .ForMember(model => model.WebSite, options => options.MapFrom(dto => dto.WebSite))
                .ForMember(model => model.Notes, options => options.MapFrom(dto => dto.Notes))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId))
                ;
        }
    }
}
