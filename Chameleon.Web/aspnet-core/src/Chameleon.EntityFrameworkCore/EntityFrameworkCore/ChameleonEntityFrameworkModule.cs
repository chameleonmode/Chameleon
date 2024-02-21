using Abp.Dependency;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Chameleon.Configuration;
using Chameleon.EntityFrameworkCore.Seed;
using System;

namespace Chameleon.EntityFrameworkCore
{
    [DependsOn(
        typeof(ChameleonCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class ChameleonEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<ChameleonDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        ChameleonDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        ChameleonDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ChameleonEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            var configurationAccessor = IocManager.Resolve<IAppConfigurationAccessor>();

            using (var scope = IocManager.CreateScope())
            {
                var connectionString = configurationAccessor.Configuration["ConnectionStrings:Default"];
                if (!string.IsNullOrEmpty(connectionString))
                {
                    SeedHelper.Migrate(IocManager);
                }

                if (!SkipDbSeed && scope.Resolve<DatabaseCheckHelper>().Exist(connectionString))
                {
                    SeedHelper.SeedHostDb(IocManager);
                }
            }
        }
    }
}
