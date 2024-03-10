using AutoMapper;
using Chameleon.App.Shared.Proxies;
using Chameleon.Core.Extensions;
using Chameleon.Domain.Entities.Proxy;
using Chameleon.Infrastructure.Proxies.Api;
using Chameleon.Infrastructure.Proxies.Api.Dto;
using Chameleon.Interfaces.Proxies;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.Infrastructure.Proxies
{
    public class ProxyService
        : IProxyService
    {
        private readonly IMapper _mapper;
        private readonly IProxyApi _proxyApi;

        public ProxyService(
            IMapper mapper,
            IProxyApi proxyApi
            )
        {
            _mapper = mapper;
            _proxyApi = proxyApi;
        }

        public IList<IProxyAccess> GetAccess(ProxyAccessRequestDto input)
        {
            input.CountryId = CurrentCountry?.Id;
            var dtos = _proxyApi.GetAccess(input);
            var entities = _mapper.Map<IList<ProxyAccess>>(dtos);
            return entities.Cast<IProxyAccess>().ToList();
        }

        public IProxyCountry CurrentCountry { get; set; }

        private IList<IProxyCountry> _countries;
        public IList<IProxyCountry> GetCountries()
        {
            if (_countries != null)
            {
                return _countries;
            }

            var countries = _proxyApi
                .GetCountries()
                .ToList();

            var randomCountry = new ProxyCountryDto
            {
                Name = "Random Country"
            };

            countries.Insert(0, randomCountry);
            _countries = new List<IProxyCountry>(countries);
            // _countries = _mapper.Map<IList<IProxyCountry>>(countries);
           // _countries.AddRange(countries);
            return _countries;
        }
    }
}
