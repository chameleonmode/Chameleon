using AutoMapper;
using Chameleon.Interfaces.Proxies;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Proxies.Api.Dto
{
    public class ProxyCountryMapProfile : Profile
    {
        public ProxyCountryMapProfile()
        {
            var map = CreateMap<ProxyCountryDto, IProxyCountry>();

            map.ReverseMap();

        }
    }
}
