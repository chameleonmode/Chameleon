using Abp.Application.Services;
using Chameleon.MultiTenancy.Dto;

namespace Chameleon.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

