using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;
using Chameleon.App.Entities;
using Chameleon.App.Entities.ShareFolders;

namespace Chameleon.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool TookGuidedTour { get; set; }
        public virtual ICollection<License> Licenses { get; set; }
        public virtual ICollection<ProfileAssistant> ProfilesAssistants { get; set; }
        public virtual ICollection<UserFolder> UserFolders { get; set; }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }

        public static User CreateTenantUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = emailAddress,
                EmailAddress = emailAddress,
                Name="",
                Surname="",
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
