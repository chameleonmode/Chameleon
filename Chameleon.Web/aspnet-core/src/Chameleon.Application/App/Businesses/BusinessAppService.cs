using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class BusinessAppService
        : AsyncCrudAppService<
            Business,
            BusinessDto,
            int,
            BusinessGetAllRequestDto,
            CreateBusinessDto,
            UpdateBusinessDto
            >
        , IBusinessAppService
    {
        public BusinessAppService(
            IRepository<Business> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<Business> CreateFilteredQuery(BusinessGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<Business> ApplySorting(IQueryable<Business> query, BusinessGetAllRequestDto input)
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
