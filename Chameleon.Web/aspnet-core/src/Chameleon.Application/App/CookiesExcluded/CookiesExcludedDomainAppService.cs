using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class CookiesExcludedDomainAppService
        : AsyncCrudAppService<
            CookiesExcludedDomain,
            CookiesExcludedDomainDto,
            int,
            CookiesExcludedDomainGetAllRequestDto,
            CreateCookiesExcludedDomainDto,
            UpdateCookiesExcludedDomainDto
            >
        , ICookiesExcludedDomainAppService
    {
        public CookiesExcludedDomainAppService(
            IRepository<CookiesExcludedDomain> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<CookiesExcludedDomain> CreateFilteredQuery(CookiesExcludedDomainGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<CookiesExcludedDomain> ApplySorting(IQueryable<CookiesExcludedDomain> query, CookiesExcludedDomainGetAllRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }
    }
}
