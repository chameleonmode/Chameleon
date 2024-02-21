using Chameleon.Users.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Chameleon.App.Users
{
    public static class UserRoles
    {
        public static void AddUserRole(this CreateUserDto input, string currentUserRole)
        {
            var allRoles = new List<string>();
            if (input.RoleNames != null)
            {
                allRoles.AddRange(input.RoleNames);
            }
            allRoles.Add(currentUserRole);
            input.RoleNames = allRoles.Distinct().ToArray();
        }
    }
}
