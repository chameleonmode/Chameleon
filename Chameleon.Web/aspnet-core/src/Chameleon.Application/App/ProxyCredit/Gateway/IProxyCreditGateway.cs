using Abp.Dependency;
using Chameleon.App.Entities;
using Chameleon.Authorization.Users;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IProxyCreditGateway
        : ITransientDependency
    {
        Task<decimal> GetBalanceAsync();
        Task<decimal> GetBalanceAsync(User user);

        Task<ProxyCredit> GiveBalanceAsync(decimal amount);
        Task<ProxyCredit> GiveBalanceAsync(User user, decimal amount);
        
        Task<ProxyCredit> TakeBalanceAsync(decimal amount);
        Task<ProxyCredit> TakeBalanceAsync(User user, decimal amount);
    }
}
