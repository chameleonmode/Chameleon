using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Chameleon.EntityFrameworkCore;
using Chameleon.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Chameleon.Web.Tests
{
    [DependsOn(
        typeof(ChameleonWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class ChameleonWebTestModule : AbpModule
    {
        public ChameleonWebTestModule(ChameleonEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ChameleonWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ChameleonWebMvcModule).Assembly);
        }
    }
}