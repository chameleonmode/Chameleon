using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class OutReachTemplateAppService
        : AsyncCrudAppService<
            OutReachTemplate,
            OutReachTemplateDto,
            int,
            OutReachTemplateGetAllRequestDto,
            CreateOutReachTemplateDto,
            UpdateOutReachTemplateDto
            >
        , IOutReachTemplateAppService
    {
        public OutReachTemplateAppService(
            IRepository<OutReachTemplate> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<OutReachTemplate> CreateFilteredQuery(OutReachTemplateGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByCreatorUserId(AbpSession);
            return query;
        }

        protected override IQueryable<OutReachTemplate> ApplySorting(IQueryable<OutReachTemplate> query
            , OutReachTemplateGetAllRequestDto input)
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
