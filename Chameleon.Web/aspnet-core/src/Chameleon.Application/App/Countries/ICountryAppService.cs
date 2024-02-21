using Abp.Application.Services;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface ICountryAppService
        : IApplicationService
    {
        Task<CountryDto[]> GetAll();
    }
}
