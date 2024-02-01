using AutoMapper;
using Chameleon.Interfaces.Country;

namespace Chameleon.Infrastructure.Country.Api.Dto
{
    public class CountryMapProfile : Profile
    {
        public CountryMapProfile()
        {
            var map = CreateMap<CountryDto, ICountry>();

            map.ReverseMap();
        }
    }
}
