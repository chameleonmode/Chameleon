using Chameleon.EntityFrameworkCore.Seed.App;

namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public class InitialAppDbBuilder
    {
        private readonly ChameleonDbContext _context;

        public InitialAppDbBuilder(ChameleonDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new CountriesCreator(_context).Create();
            new WebBrowserUserAgentsCreator(_context).Create();
            new ProxyCreditPlansCreator(_context).Create();
            new ProfilePermissionCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
