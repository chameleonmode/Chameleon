using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class PersonAppService
        : AsyncCrudAppService<
            Person,
            PersonDto,
            int,
            PersonGetAllRequestDto,
            CreatePersonDto,
            UpdatePersonDto
            >
        , IPersonAppService
    {
        public PersonAppService(
            IRepository<Person> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<Person> CreateFilteredQuery(PersonGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<Person> ApplySorting(IQueryable<Person> query, PersonGetAllRequestDto input)
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
