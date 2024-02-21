using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class AddressAppService
        : AsyncCrudAppService<
            Address,
            AddressDto,
            int,
            AddressGetAllRequestDto,
            CreateAddressDto,
            UpdateAddressDto
            >
        , IAddressAppService
    {
        public AddressAppService(
            IRepository<Address> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<Address> CreateFilteredQuery(AddressGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.FilterByMustHaveProfile(input);
            return query;
        }

        protected override IQueryable<Address> ApplySorting(IQueryable<Address> query, AddressGetAllRequestDto input)
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
