using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Chameleon.Configuration;
using Chameleon.Web;

namespace Chameleon.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ChameleonDbContextFactory : IDesignTimeDbContextFactory<ChameleonDbContext>
    {
        public ChameleonDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ChameleonDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ChameleonDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ChameleonConsts.ConnectionStringName));

            return new ChameleonDbContext(builder.Options);
        }
    }
}
