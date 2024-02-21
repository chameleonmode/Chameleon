using System.Threading.Tasks;
using Abp.Application.Services;
using Chameleon.Sessions.Dto;

namespace Chameleon.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
