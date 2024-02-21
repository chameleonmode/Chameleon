using System.Threading.Tasks;
using Abp.Application.Services;
using Chameleon.Authorization.Accounts.Dto;

namespace Chameleon.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
