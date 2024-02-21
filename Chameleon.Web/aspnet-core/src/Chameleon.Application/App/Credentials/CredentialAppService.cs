using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class CredentialAppService
        : AsyncCrudAppService<
            Credential,
            CredentialDto,
            int,
            CredentialGetAllRequestDto,
            CreateCredentialDto,
            UpdateCredentialDto
            >
        , ICredentialAppService
    {
        public CredentialAppService(
            IRepository<Credential> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<Credential> CreateFilteredQuery(CredentialGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<Credential> ApplySorting(IQueryable<Credential> query, CredentialGetAllRequestDto input)
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
