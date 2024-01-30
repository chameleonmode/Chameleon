using Chameleon.Interfaces.UserProfiles;
using System.Linq;

namespace Chameleon.Interfaces.App.UserProfiles.Extensions
{
    public static class IUserProfileExtensions
    {
        public static bool HasPermission(this IUserProfile profile, string permissionName)
        {
            return profile.PermissionNames.Contains(permissionName);
        }
    }
}
