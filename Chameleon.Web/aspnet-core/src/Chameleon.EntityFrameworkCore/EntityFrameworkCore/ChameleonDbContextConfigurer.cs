using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Chameleon.EntityFrameworkCore
{
    public static class ChameleonDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ChameleonDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public static void Configure(DbContextOptionsBuilder<ChameleonDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, ServerVersion.AutoDetect(connection.ConnectionString));
        }
    }
}
