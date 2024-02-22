using AutoMapper;
using Chameleon.Infrastructure.Country.Api;
using Chameleon.Infrastructure.Country.Api.Dto;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Country;


namespace Chameleon.Infrastructure.Country.Repository
{
    public class CountryRepository 
        : Repository<Domain.Entities.Country.Country,
            ICountry,
            CountryDto
            >
        , ICountryRepository
    {
        public CountryRepository(
            IMapper mapper,
            ICountryApi apiClient,
            IEventAggregator eventAggregator
            ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
