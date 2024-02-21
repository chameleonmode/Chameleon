using Abp.Application.Services;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IChangeLogAppService
        : IApplicationService
    {
        Task<object> GetAll();
    }
}
