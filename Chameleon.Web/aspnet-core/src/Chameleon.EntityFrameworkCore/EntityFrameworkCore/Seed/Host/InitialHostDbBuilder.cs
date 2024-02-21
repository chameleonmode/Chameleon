using Chameleon.EntityFrameworkCore.Seed.App;

namespace Chameleon.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly ChameleonDbContext _context;

        public InitialHostDbBuilder(ChameleonDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new UserAppCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
