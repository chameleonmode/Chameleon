using Abp.Application.Services;

namespace Chameleon.App
{
    public interface ICookiesExcludedDomainAppService
        : IAsyncCrudAppService<
            CookiesExcludedDomainDto,
            int,
            CookiesExcludedDomainGetAllRequestDto,
            CreateCookiesExcludedDomainDto,
            UpdateCookiesExcludedDomainDto
            >
    {
    }
}
