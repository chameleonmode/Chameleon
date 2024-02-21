using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.UI;
using Abp.Zero.Configuration;
using Chameleon.App.Entities;
using Chameleon.App.Services.License_Key.Dto;
using Chameleon.App.ValueObjects;
using Chameleon.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.Editions;
using Chameleon.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Chameleon.App.Services.License_Key
{
    public class LicenseKeyService : ILicenseKeyService
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly EditionManager _editionManager;
        private readonly IRepository<License> _repository;
        private readonly ILocalizationManager _localizationManager;
        private readonly IRepository<Role> _roleRepositoty;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly IFeatureDependencyContext _featureDependencyContext;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<PermissionSetting, long> _rolePermissionSettingRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

        public LicenseKeyService(
            IRepository<License> repository,
            ILocalizationManager localizationManager,
            UserManager userManager,
            EditionManager editionManager,
            IRepository<Tenant> tenantRepository,
            IRepository<Role> roleRepositoty,
            IRoleManagementConfig roleManagementConfig,
            IFeatureDependencyContext featureDependencyContext,
            IPermissionManager permissionManager,
            IRepository<PermissionSetting, long> rolePermissionSettingRepository,
            IRepository<UserRole, long> userRoleRepository
            )
        {
            _repository = repository;
            _localizationManager = localizationManager;
            _userManager = userManager;
            _editionManager = editionManager;
            _tenantRepository = tenantRepository;
            _roleRepositoty = roleRepositoty;
            _roleManagementConfig = roleManagementConfig;
            _featureDependencyContext = featureDependencyContext;
            _permissionManager = permissionManager;
            _rolePermissionSettingRepository = rolePermissionSettingRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<License> GetOrCreateAsync(string emailAddress, string licenseKeyValue)
        {
            await _lock.WaitAsync();
            try
            {
                return await GetOrCreateInternalAsync(emailAddress, licenseKeyValue);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<License> GetOrCreateInternalAsync(string emailAddress, string licenseKeyValue)
        {
            var user = await FindOrRegisterAsync(
                emailAddress,
                User.DefaultPassword
            );

            var licenseKey = LicenseKey.Create(licenseKeyValue);

            var license = _repository.GetAll()
                .FirstOrDefault(entity => entity.LicenseKeyValue == licenseKey.Value);

            if (license != null)
            {
                if (license.UserId != user.Id)
                {
                    throw new UserFriendlyException($"License Key {licenseKey.Value} is already assigned to another user.");
                }
                return license;
            }

            license = new License
            {
                UserId = user.Id,
                LicenseKey = licenseKey
            };

            license.Id = await _repository.InsertAndGetIdAsync(license);
            await CurrentUnitOfWork.SaveChangesAsync();

            return license;
        }

        private async Task<User> FindOrRegisterAsync(string emailAddress, string plainPassword)
        {
            var user = await _userManager.FindByNameOrEmailAsync(emailAddress);

            if (user == null)
            {
                return await CreateTenantWithUserAsync(emailAddress, plainPassword);
            }

            if (user.TenantId.HasValue)
            {
                return user;
            }

            return await CreateTenantForUserAsync(emailAddress, (tenantId) =>
            {
                user.TenantId = tenantId;
                return Task.FromResult(user);
            });
        }

        private async Task<User> CreateTenantWithUserAsync(string emailAddress, string plainPassword)
        {
            return await CreateTenantForUserAsync(emailAddress, async (tenantId) =>
            {
                // Create admin user for the tenant
                var adminUser = User.CreateTenantUser(tenantId, emailAddress);
                await _userManager.InitializeOptionsAsync(tenantId);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id
                return adminUser;
            });
        }

        private async Task<User> CreateTenantForUserAsync(string emailAddress, Func<int, Task<User>> getUser)
        {
            var tenantId = await CreateTenant(emailAddress);
            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                // Create static roles for new tenant
                var staticRoleDefinitions = _roleManagementConfig.StaticRoles.Where(
                   sr => sr.Side == MultiTenancySides.Tenant
                );

                foreach (var staticRoleDefinition in staticRoleDefinitions)
                {
                    var role = new Role
                    {
                        TenantId = tenantId,
                        Name = staticRoleDefinition.RoleName,
                        DisplayName = staticRoleDefinition.RoleDisplayName,
                        IsStatic = true
                    };

                    role.SetNormalizedName();
                    await _roleRepositoty.InsertAsync(role);
                }

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = await _roleRepositoty
                   .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == StaticRoleNames.Tenants.Admin);

                var permissionsByRole = _permissionManager.GetAllPermissions(adminRole.GetMultiTenancySide())
                    .Where(x => x.FeatureDependency == null
                        || x.FeatureDependency.IsSatisfied(_featureDependencyContext));

                foreach (var permission in permissionsByRole)
                {
                    await _rolePermissionSettingRepository.InsertAsync(new RolePermissionSetting
                    {
                        TenantId = tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    });
                }

                // Create admin user for the tenant
                var adminUser = await getUser(tenantId);

                // Assign admin user to role!
                await _userRoleRepository.InsertAsync(new UserRole
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id,
                    TenantId = tenantId
                });

                //Create assistant role
                var assistantRole = new Role
                {
                    TenantId = tenantId,
                    Name = StaticRoleNames.Tenants.AssistantUser,
                    DisplayName = StaticRoleNames.Tenants.AssistantUser,
                    IsStatic = true
                };

                assistantRole.SetNormalizedName();
                await _roleRepositoty.InsertAsync(assistantRole);

                await _userRoleRepository.InsertAsync(new UserRole
                {
                    RoleId = assistantRole.Id,
                    UserId = adminUser.Id,
                    TenantId = tenantId
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                await _rolePermissionSettingRepository.InsertAsync(new RolePermissionSetting
                {
                    TenantId = tenantId,
                    Name = PermissionNames.Pages_Users_Assistant,
                    IsGranted = true,
                    RoleId = assistantRole.Id
                });

                await _rolePermissionSettingRepository.InsertAsync(new RolePermissionSetting
                {
                    TenantId = tenantId,
                    Name = PermissionNames.Pages_AssistantUsers,
                    IsGranted = true,
                    RoleId = assistantRole.Id
                });

                await CurrentUnitOfWork.SaveChangesAsync();

                return adminUser;
            }
        }

        public async Task<int> CreateTenant(string emailAddress)
        {
            var tenant = new Tenant(emailAddress, emailAddress);
            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);

            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantRepository.InsertAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            return tenant.Id;
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(_localizationManager);
        }

        public async Task<bool> IsValidAsync(string licenseKey)
        {
            if (LicenseKey.IsValid(licenseKey))
            {
                return await IsValidAsync(LicenseKey.Create(licenseKey));
            }
            return false;
        }

        public async Task<bool> IsValidAsync(LicenseKey licenseKey)
        {
            try
            {
                return await IsValidInternalAsync(licenseKey);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<LicenseType> GetTypeAsync(LicenseKey licenseKey)
        {
            var responseObject = await GetAsync<LicenseDataResponse>(licenseKey, "data");

            if (responseObject.Data == null)
            {
                return LicenseType.NONE;
            }

            return responseObject.Data.ProductId;
        }

        private async Task<bool> IsValidInternalAsync(LicenseKey licenseKey)
        {
            var responseObject = await GetAsync<LicenseStatusReponse>(licenseKey, "status");

            return responseObject.Success &&
                   responseObject.Data != null &&
                   responseObject.Data.Valid &&
                   responseObject.Data.Active;
        }

        private async Task<T> GetAsync<T>(LicenseKey licenseKey, string action)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(GetUrl(licenseKey, action));
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(jsonString);

                return result;
            }
        }

        private string GetUrl(LicenseKey licenseKey, string action)
        {
            return $"https://app.paykickstart.com/api/licenses/{action}?auth_token=iACWLSJEf1Gj&license_key={licenseKey.Value}";
        }
    }
}
