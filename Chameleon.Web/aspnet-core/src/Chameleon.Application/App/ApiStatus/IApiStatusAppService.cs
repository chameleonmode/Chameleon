using Abp.Application.Services;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IApiStatusAppService
        : IApplicationService
    {
        Task GetStatusAsync();
    }
}
