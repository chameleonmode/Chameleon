using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using Chameleon.Authorization;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize, AbpAuthorize(PermissionNames.Pages_Prospector)]
    public class ProspectorBlogsOfInterestAppService
        : AsyncCrudAppService<
            ProspectorBlogsOfInterest,
            ProspectorBlogsOfInterestDto,
            int,
            ProspectorBlogsOfInterestGetAllRequestDto,
            CreateProspectorBlogsOfInterestDto,
            UpdateProspectorBlogsOfInterestDto
            >
        , IProspectorBlogsOfInterestAppService
    {

        public ProspectorBlogsOfInterestAppService(IRepository<ProspectorBlogsOfInterest> repository)
            : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<ProspectorBlogsOfInterest> CreateFilteredQuery(
            ProspectorBlogsOfInterestGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            query = query.FilterByCreatorUserId(AbpSession);
            return query;
        }

        protected override IQueryable<ProspectorBlogsOfInterest> ApplySorting(
            IQueryable<ProspectorBlogsOfInterest> query, ProspectorBlogsOfInterestGetAllRequestDto input)
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
