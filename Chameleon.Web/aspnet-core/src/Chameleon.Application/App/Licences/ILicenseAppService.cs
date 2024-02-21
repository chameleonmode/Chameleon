using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface ILicenseAppService
        : IApplicationService
    {
        Task<PagedResultDto<LicenseDto>> GetAllAsync(LicenseGetAllRequestDto input);
    }
}
