using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using Chameleon.Authorization;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize/*, AbpAuthorize(PermissionNames.Pages_Outreach)*/]
    public class OutReachLinkAppService
        : AsyncCrudAppService<
            OutReachLink,
            OutReachLinkDto,
            int,
            OutReachLinkGetAllRequestDto,
            CreateOutReachLinkDto,
            UpdateOutReachLinkDto
            >
        , IOutReachLinkAppService
    {
        public OutReachLinkAppService(
           IRepository<OutReachLink> repository
           ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<OutReachLink> CreateFilteredQuery(OutReachLinkGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            query = query.FilterByCreatorUserId(AbpSession);
            return query;
        }

        protected override IQueryable<OutReachLink> ApplySorting(IQueryable<OutReachLink> query, OutReachLinkGetAllRequestDto input)
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
