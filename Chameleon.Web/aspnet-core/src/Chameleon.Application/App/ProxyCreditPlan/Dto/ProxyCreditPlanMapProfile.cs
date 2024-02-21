using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class ProxyCreditPlanMapProfile : AutoMapper.Profile
    {
        public ProxyCreditPlanMapProfile()
        {
            CreateEntityDtoMap<ProxyCreditPlanDto>()
                .ReverseMap()
                ;

            CreateBaseDtoMap<CreateProxyCreditPlanDto>();
            CreateEntityDtoMap<UpdateProxyCreditPlanDto>();
        }

        private IMappingExpression<TDto, ProxyCreditPlan> CreateEntityDtoMap<TDto>()
            where TDto : ProxyCreditPlanEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, ProxyCreditPlan> CreateBaseDtoMap<TDto>()
            where TDto : ProxyCreditPlanBaseDto
        {
            return CreateMap<TDto, ProxyCreditPlan>()
                .ForMember(dto => dto.Amount, options => options.MapFrom(dto => dto.Amount))
                .ForMember(dto => dto.Title, options => options.MapFrom(dto => dto.Title))
                ;
        }
    }
}
