using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Chameleon.App.Entities;
using Chameleon.Authorization;

namespace Chameleon.App
{
    [AbpAuthorize(PermissionNames.Pages_ProxyCreditPlans)]
    public partial class ProxyCreditPlanAppService
        : AsyncCrudAppService<
            ProxyCreditPlan,
            ProxyCreditPlanDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateProxyCreditPlanDto,
            UpdateProxyCreditPlanDto
            >
        , IProxyCreditPlanAppService
    {
        public ProxyCreditPlanAppService(
            IRepository<ProxyCreditPlan> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }
    }
}
