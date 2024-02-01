using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Domain.Entities.Proxy;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Infrastructure.Proxies.Api.Dto
{
    public class ProxyMapProfile : Profile
    {
        public ProxyMapProfile()
        {
            CreateProxySettingsMap();
            CreateProxyAccessMap();
        }

        private void CreateProxySettingsMap()
        {
            CreateMap<ProxyDto, IProxySettings>()
                .ConstructUsing(dto => new ProxySettings())
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                .ForMember(model => model.Host, options => options.MapFrom(dto => dto.Host))
                .ForMember(model => model.Port, options => options.MapFrom(dto => dto.Port))
                .ForAllOtherMembers(options => options.Ignore());

            CreateMap<IProxySettings, ProxyDto>()
                .ForMember(dto => dto.UserName, options => options.MapFrom(model => model.UserName))
                .ForMember(dto => dto.Password, options => options.MapFrom(model => model.Password))
                .ForMember(dto => dto.Host, options => options.MapFrom(model => model.Host))
                .ForMember(dto => dto.Port, options => options.MapFrom(model => model.Port))
                .ForAllOtherMembers(options => options.Ignore());
        }

        private void CreateProxyAccessMap()
        {
            CreateMap<ProxyAccessDto, ProxyAccess>()
                .ForMember(model => model.UserName, options => options.MapFrom(dto => dto.UserName))
                .ForMember(model => model.Password, options => options.MapFrom(dto => dto.Password))
                .ForMember(model => model.Host, options => options.MapFrom(dto => dto.Host))
                .ForMember(model => model.Port, options => options.MapFrom(dto => dto.Port))
                .ForMember(model => model.Url, options => options.MapFrom(dto => dto.Url))
                .ForAllOtherMembers(options => options.Ignore());
        }
    }
}
