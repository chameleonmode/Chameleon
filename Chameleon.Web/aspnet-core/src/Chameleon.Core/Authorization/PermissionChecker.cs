using Abp.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;

namespace Chameleon.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
