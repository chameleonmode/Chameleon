using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public interface IProxyCreditPlanAppService
        : IAsyncCrudAppService<
            ProxyCreditPlanDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateProxyCreditPlanDto,
            UpdateProxyCreditPlanDto
            >
    {
    }
}
