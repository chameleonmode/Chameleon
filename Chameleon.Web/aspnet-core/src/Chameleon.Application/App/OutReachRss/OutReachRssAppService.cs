using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class OutReachRssAppService
        : AsyncCrudAppService<
            OutReachRss,
            OutReachRssDto,
            int,
            OutReachRssGetAllRequestDto,
            CreateOutReachRssDto,
            UpdateOutReachRssDto
            >
        , IOutReachRssAppService
    {
        public OutReachRssAppService(
            IRepository<OutReachRss> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<OutReachRss> CreateFilteredQuery(OutReachRssGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            query = query.FilterByCreatorUserId(AbpSession);
            return query;
        }

        protected override IQueryable<OutReachRss> ApplySorting(IQueryable<OutReachRss> query, OutReachRssGetAllRequestDto input)
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
