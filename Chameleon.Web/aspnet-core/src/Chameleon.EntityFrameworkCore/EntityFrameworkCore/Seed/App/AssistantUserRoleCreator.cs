using Chameleon.Authorization.Roles;
using Chameleon.EntityFrameworkCore.Seed.Host.App;
using System.Linq;

namespace Chameleon.EntityFrameworkCore.Seed.App
{
    public class AssistantUserRoleCreator : ApplicationBaseCreator
    {
        public AssistantUserRoleCreator(ChameleonDbContext context)
            : base(context)
        {
        }

        public override void Run()
        {
            if (Context.Roles.Any(role => role.Name == StaticRoleNames.Tenants.AssistantUser)) 
            {
                return;
            }

            var role = new Role
            {
                Name = StaticRoleNames.Tenants.AssistantUser,
                DisplayName = StaticRoleNames.Tenants.AssistantUser,
                NormalizedName = StaticRoleNames.Tenants.AssistantUser.ToUpper(),
                IsStatic = true,
                IsDefault = false,
            };

            Context.Roles.Add(role);
            SaveChanges();
        }
    }
}
