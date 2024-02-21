using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Chameleon.App.Entities;
using Chameleon.App.Services;
using Chameleon.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    /*
    [AbpAuthorize(PermissionNames.Pages_Licences)]
    public class LicenceAppService
        : AsyncCrudAppService<
            License,
            LicenseDto,
            int,
            LicenseGetAllRequestDto,
            CreateLicenseDto,
            UpdateLicenseDto
            >
        , ILicenseAppService
    {
        private readonly ILicenseKeyService _licenseKeyService;

        public LicenceAppService(
            IRepository<License> repository,
            ILicenseKeyService licenseKeyService
            ) : base(repository)
        {
            _licenseKeyService = licenseKeyService;
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<License> CreateFilteredQuery(LicenseGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            //query = query.FilterByCreatorUserId(AbpSession);
            return query;
        }

        public override Task<LicenseDto> CreateAsync(CreateLicenseDto input)
        {
            return CreateAsync(input.EmailAddress, input.LicenseKey);
        }

        private async Task<LicenseDto> CreateAsync(string emailAddress, string licenseKeyValue = null)
        {
            var license = await _licenseKeyService.CreateAsync(
                emailAddress, licenseKeyValue, 
                AbpSession.TenantId
                );

            return new LicenseDto
            {
                Id = license.Id,
                LicenseKey = license.LicenseKeyValue,
            };
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAsync(input.Id);

            await Repository.DeleteAsync(entity);
        }

        public override Task<LicenseDto> UpdateAsync(UpdateLicenseDto input)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<License> ApplySorting(IQueryable<License> query, LicenseGetAllRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }
    }
    */
}
