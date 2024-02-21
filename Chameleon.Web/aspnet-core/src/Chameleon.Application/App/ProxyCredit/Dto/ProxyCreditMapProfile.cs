using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class ProxyCreditMapProfile : AutoMapper.Profile
    {
        public ProxyCreditMapProfile()
        {
            CreateEntityDtoMap<ProxyCreditDto>()
                .ReverseMap()
                .ForMember(dto => dto.Amount, opts => opts.Ignore())
                ;

            CreateBaseDtoMap<ProxyCreditBaseDto>()
                .ReverseMap()
                .ForMember(dto => dto.Amount, opts => opts.Ignore());

            CreateBaseDtoMap<CreateProxyCreditDto>();
            CreateEntityDtoMap<UpdateProxyCreditDto>();
        }

        private IMappingExpression<TDto, ProxyCredit> CreateEntityDtoMap<TDto>()
            where TDto : ProxyCreditEntityDto
        {
            return CreateBaseDtoMap<TDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                ;
        }

        private IMappingExpression<TDto, ProxyCredit> CreateBaseDtoMap<TDto>()
            where TDto : ProxyCreditBaseDto
        {
            return CreateMap<TDto, ProxyCredit>();
        }
    }
}
