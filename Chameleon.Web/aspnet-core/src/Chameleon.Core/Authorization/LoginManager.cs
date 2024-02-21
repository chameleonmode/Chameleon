using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.MultiTenancy;
using System.Threading.Tasks;
using Abp.Extensions;
using System;
using System.Linq;

namespace Chameleon.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        public LogInManager(
            UserManager userManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager, 
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository, 
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher, 
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory) 
            : base(
                  userManager, 
                  multiTenancyConfig,
                  tenantRepository, 
                  unitOfWorkManager, 
                  settingManager, 
                  userLoginAttemptRepository, 
                  userManagementConfig, 
                  iocResolver, 
                  passwordHasher, 
                  roleManager, 
                  claimsPrincipalFactory)
        {
        }

        [UnitOfWork]
        public virtual async Task<AbpLoginResult<Tenant, User>> LoginAsync(string userNameOrEmailAddress, string tenancyName = null, bool shouldLockout = true)
        {
            var result = await LoginAsyncInternal(userNameOrEmailAddress, tenancyName, shouldLockout);
            SaveLoginAttempt(result, tenancyName, userNameOrEmailAddress);
            return result;
        }

        protected virtual async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternal(string userNameOrEmailAddress, string tenancyName, bool shouldLockout)
        {
            if (userNameOrEmailAddress.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userNameOrEmailAddress));
            }

            Tenant tenant = null;

            using (UnitOfWorkManager.Current.SetTenantId(null))
            {
                if (!MultiTenancyConfig.IsEnabled)
                {
                    tenant = await GetDefaultTenantAsync();
                }
                else if (!tenancyName.IsNullOrEmpty())
                {
                    tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);

                    if (tenant == null)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                    }

                    if (!tenant.IsActive)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                    }
                }
            }

            var tenantId = tenant == null ? (int?)null : tenant.Id;

            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var user = UserManager.Users
                    .FirstOrDefault(u => u.TenantId == tenantId && (u.UserName == userNameOrEmailAddress || u.EmailAddress == userNameOrEmailAddress));

                if (user == null)
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, tenant);
                }

                if (await UserManager.IsLockedOutAsync(user))
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                }

                return await CreateLoginResultAsync(user, tenant);
            }
        }
    }
}
