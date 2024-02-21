using Chameleon.App.Entities.Permissions;
using Chameleon.Authorization;
using Chameleon.EntityFrameworkCore.Seed.Host.App;

namespace Chameleon.EntityFrameworkCore.Seed.App
{
    public class ProfilePermissionCreator : ApplicationBaseCreator
    {
        private static readonly ProfilePermission[] _permissions = new ProfilePermission[]
        {
            new ProfilePermission
            {
                PermissionName = PermissionNames.Pages_Curate,
                DisplayName = "Curate",
            },
            new ProfilePermission
            {
                PermissionName = PermissionNames.Pages_RSS,
                DisplayName = "RSS",
            },
            new ProfilePermission
            {
                PermissionName = PermissionNames.Pages_YouTube,
                DisplayName = "YT Uploader",
            },
            new ProfilePermission
            {
                PermissionName = PermissionNames.Pages_Prospector,
                DisplayName = "Prospector",
            },
            new ProfilePermission
            {
                PermissionName = PermissionNames.Pages_Outreach,
                DisplayName = "Outreach",
            }
        };

        public ProfilePermissionCreator(ChameleonDbContext context)
            : base(context)
        {
        }
        public override void Run()
        {
            foreach (var item in _permissions)
            {
                Create(item);
            }

            SaveChanges();
        }

        private void Create(ProfilePermission permission)
        {
            var table = Context.ProfilePermissions;

            foreach(var record in table)
            {
                if (record.PermissionName == permission.PermissionName
                    && record.DisplayName == permission.DisplayName)
                {
                    return;
                }

                if(record.PermissionName == permission.PermissionName
                    && string.IsNullOrEmpty(record.DisplayName))
                {
                    record.DisplayName = permission.DisplayName;
                    return;
                }

                if(record.DisplayName == permission.DisplayName
                    && string.IsNullOrEmpty(record.PermissionName))
                {
                    record.PermissionName = permission.PermissionName;
                    return;
                }
            }

            table.Add(permission);
        }
    }
}
