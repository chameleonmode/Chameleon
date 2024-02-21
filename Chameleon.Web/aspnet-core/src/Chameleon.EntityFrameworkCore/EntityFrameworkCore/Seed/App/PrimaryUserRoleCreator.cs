using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Chameleon.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.EntityFrameworkCore.Seed.Host.App;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Chameleon.EntityFrameworkCore.Seed.App
{
    public class PrimaryUserRoleCreator : ApplicationBaseCreator
    {
        private static readonly string[] IgnorePerrmisionNames = new[]
        {
            PermissionNames.Pages_Tenants,
            PermissionNames.Pages_Users_Activation,
            PermissionNames.Pages_Roles,
            PermissionNames.Pages_Licences
        };

        public PrimaryUserRoleCreator(ChameleonDbContext context)
            : base(context)
        {
        }

        private Role _role;
        public override void Run()
        {
            _role = Context.Roles.FirstOrDefault(role => role.Name == StaticRoleNames.Tenants.PrimaryUser);
            if (_role == null)
            {
                _role = CreateRole();
            }

            GrantedPermissions();
        }

        private Role CreateRole()
        {
            var role = new Role
            {
                Name = StaticRoleNames.Tenants.PrimaryUser,
                DisplayName = StaticRoleNames.Tenants.PrimaryUser,
                NormalizedName = StaticRoleNames.Tenants.PrimaryUser.ToUpper(),
                IsStatic = true,
                IsDefault = true
            };

            Context.Roles.Add(role);
            SaveChanges();
            AssignRoleToExistingUsers(role);
            return role;
        }

        private void AssignRoleToExistingUsers(Role role)
        {
            var users = Context.Users
                .Where(u => u.Name != AbpUserBase.AdminUserName)
                .Select(u => new
                {
                    Id = u.Id,
                    TenantId = u.TenantId,
                })
                .ToList()
                ;

            foreach (var user in users)
            {
                Context.UserRoles.Add(new UserRole(user.TenantId, user.Id, role.Id));
            }
        }

        private void GrantedPermissions()
        {
            var grantedPermissions = Context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == null && p.RoleId == _role.Id)              
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new ChameleonAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) && !grantedPermissions.Contains(p.Name))
                .Where(p => !IgnorePerrmisionNames.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                Context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = null,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = _role.Id
                    })
                );
                Context.SaveChanges();
            }
        }
    }
}
