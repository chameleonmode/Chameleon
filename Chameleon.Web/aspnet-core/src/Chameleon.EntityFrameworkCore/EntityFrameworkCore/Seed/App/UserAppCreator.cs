using Chameleon.EntityFrameworkCore.Seed.Host.App;

namespace Chameleon.EntityFrameworkCore.Seed.App
{

    public class UserAppCreator : ApplicationBaseCreator
    {
        public UserAppCreator(ChameleonDbContext context)
            : base(context)
        {

        }

        public override void Run()
        {
            new PrimaryUserRoleCreator(Context).Run();
            new AssistantUserRoleCreator(Context).Run();
        }
    }
}
