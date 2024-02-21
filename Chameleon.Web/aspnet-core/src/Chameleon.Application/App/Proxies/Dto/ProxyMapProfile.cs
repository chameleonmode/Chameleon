using AutoMapper;
using Chameleon.App.Entities;

namespace Chameleon.App.Dto
{
    public class ProxyMapProfile : AutoMapper.Profile
    {
        public ProxyMapProfile()
        {
            CreateBaseDtoMap<ProxyDto>()
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ReverseMap()
                ;

            CreateBaseDtoMap<ProxyBaseDto>()
                .ReverseMap();

            // TODO: CreateProxyDto
            // TODO: UpdateProxyDto
        }

        private IMappingExpression<TDto, Proxy> CreateBaseDtoMap<TDto>()
            where TDto : ProxyBaseDto
        {
            return CreateMap<TDto, Proxy>()
                .ForMember(model => model.Host, options => options.MapFrom(dto => dto.Host))
                .ForMember(model => model.Port, options => options.MapFrom(dto => dto.Port))
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                ;
        }
    }
}
