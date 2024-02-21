using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class CountryAppService
        : ChameleonAppServiceBase
        , ICountryAppService
    {
        private readonly IRepository<Country> _repository;

        public CountryAppService(
            IRepository<Country> repository
            )
        {
            _repository = repository;
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public Task<CountryDto[]> GetAll()
        {
            var entities = _repository
                .GetAll()
                .OrderBy(entity => entity.Name)
                .ToList();
            var dtos = ObjectMapper.Map<CountryDto[]>(entities);
            return Task.FromResult(dtos);
        }
    }
}
