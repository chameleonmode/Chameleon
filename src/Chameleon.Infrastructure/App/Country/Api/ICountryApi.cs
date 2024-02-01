using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Country.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.Country.Api
{
    public interface ICountryApi 
        : IApiLayer<CountryDto>
        , ISingletonDependency
    {
    }
}
