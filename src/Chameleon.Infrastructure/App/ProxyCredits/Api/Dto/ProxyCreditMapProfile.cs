using AutoMapper;
using Chameleon.Interfaces.ProxyCredit;

namespace Chameleon.Infrastructure.ProxyCredit.Api.Dto
{
    public class ProxyCreditMapProfile : Profile
    {
        public ProxyCreditMapProfile()
        {
            var map = CreateMap<ProxyCreditDto, IProxyCredit>()
                .ConstructUsing(dto => new Domain.Entities.Proxy.ProxyCredit())
                .ForMember(model => model.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(model => model.Amount, options => options.MapFrom(dto => dto.Amount))
                ;

            map.ReverseMap();
        }
    }
}
