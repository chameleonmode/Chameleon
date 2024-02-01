using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.Country.Api.Dto;
using Chameleon.Interfaces.Api;
using System;

namespace Chameleon.Infrastructure.Country.Api
{
    public class CountryApi
        : ApiLayer<CountryDto>
        , ICountryApi
    {
        public CountryApi(
            IApiClient apiClient
            ) : base(apiClient, "country")
        {
        }

        public override CountryDto Get(int id)
        {
            throw new NotImplementedException("Country API not supported GET/id request");
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException("Country API not supported DELETE request");
        }

        public override CountryDto Insert(CountryDto input)
        {
            throw new NotImplementedException("Country API not supported INSERT request");
        }

        public override void Update(CountryDto input)
        {
            throw new NotImplementedException("Country API not supported UPDATE request");
        }
    }
}
