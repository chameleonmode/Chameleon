using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class RSSFeedAppService
        : AsyncCrudAppService<
            RSSFeed,
            RSSFeedDto,
            int,
            RSSFeedGetAllRequestDto,
            CreateRSSFeedDto,
            UpdateRSSFeedDto
            >
        , IRSSFeedAppService
    {
        public RSSFeedAppService(
            IRepository<RSSFeed> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<RSSFeed> CreateFilteredQuery(RSSFeedGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<RSSFeed> ApplySorting(IQueryable<RSSFeed> query, RSSFeedGetAllRequestDto input)
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
