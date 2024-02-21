using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class OutReachTemplateMapProfile : AutoMapper.Profile
    {
        public OutReachTemplateMapProfile()
        {
            CreateEntityDtoMap<OutReachTemplateDto>()
              .ReverseMap();

            CreateBaseDtoMap<OutReachTemplateBaseDto>()
              .ReverseMap();

            CreateBaseDtoMap<CreateOutReachTemplateDto>();
            CreateEntityDtoMap<UpdateOutReachTemplateDto>();
        }

        private IMappingExpression<TDto, OutReachTemplate> CreateEntityDtoMap<TDto>()
            where TDto : OutReachTemplateEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, OutReachTemplate> CreateBaseDtoMap<TDto>()
            where TDto : OutReachTemplateBaseDto
        {
            return CreateMap<TDto, OutReachTemplate>()
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Content, options => options.MapFrom(dto => dto.Content))
                .ForMember(model => model.Subject, options => options.MapFrom(dto => dto.Subject));
        }
    }
}
