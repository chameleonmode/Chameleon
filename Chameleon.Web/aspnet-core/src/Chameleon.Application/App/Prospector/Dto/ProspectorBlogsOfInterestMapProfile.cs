using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App
{
    public class ProspectorBlogsOfInterestMapProfile : AutoMapper.Profile
    {
        public ProspectorBlogsOfInterestMapProfile()
        {
            CreateEntityDtoMap<ProspectorBlogsOfInterestDto>()
                .ReverseMap();

            CreateBaseDtoMap<ProspectorBlogsOfInterestBaseDto>()
                .ReverseMap();

            CreateBaseDtoMap<CreateProspectorBlogsOfInterestDto>();
            CreateEntityDtoMap<UpdateProspectorBlogsOfInterestDto>();
        }

        private IMappingExpression<TDto, ProspectorBlogsOfInterest> CreateEntityDtoMap<TDto>()
            where TDto : ProspectorBlogsOfInterestEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id));
        }

        private IMappingExpression<TDto, ProspectorBlogsOfInterest> CreateBaseDtoMap<TDto>()
            where TDto : ProspectorBlogsOfInterestBaseDto
        {
            return CreateMap<TDto, ProspectorBlogsOfInterest>()
                .ForMember(model => model.Name, options => options.MapFrom(dto => dto.Name))
                .ForMember(model => model.Value, options => options.MapFrom(dto => dto.Value))
                .ForMember(model => model.Type, options => options.MapFrom(dto => dto.Type))
                .ForMember(model => model.ProfileId, options => options.MapFrom(dto => dto.ProfileId));
        }
    }
}
