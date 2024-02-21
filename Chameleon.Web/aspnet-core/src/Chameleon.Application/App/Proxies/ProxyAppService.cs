using Abp.Authorization;
using Abp.Domain.Repositories;
using Chameleon.App.Dto;
using Chameleon.App.Entities;
using Chameleon.App.Shared.Proxies;
using Chameleon.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    [AbpAuthorize(PermissionNames.Pages_Proxy)]
    public class ProxyAppService 
        : ChameleonAppServiceBase
        , IProxyAppService
    {
        private readonly IRepository<ProxyCredit> _repository;
        private readonly IPacketStreamAccessBuilder _packetStreamAccessBuilder;

        public ProxyAppService(
            IRepository<ProxyCredit> repository,
            IPacketStreamAccessBuilder packetStreamAccessBuilder
            )
        {
            _repository = repository;
            _packetStreamAccessBuilder = packetStreamAccessBuilder;
        }

        public IList<ProxyCountryDto> GetCountries()
        {
            var countries = _packetStreamAccessBuilder.GetCountries();
            var countriesDto = ObjectMapper.Map<List<ProxyCountryDto>>(countries);
            return countriesDto;
        }

        public IList<ProxyAccessDto> GetAccess(ProxyAccessRequestDto input)
        {
            var proxyCredit = _repository.GetAll()
                .FilterByUserId(AbpSession)
                .FirstOrDefault();
            if (proxyCredit == null)
            {
                return new List<ProxyAccessDto>();
            }

            var result = _packetStreamAccessBuilder
                .Build(input, proxyCredit);
            return result;
        }
    }
}
