using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Chameleon.Authorization;

namespace Chameleon
{
    [DependsOn(
        typeof(ChameleonCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ChameleonApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ChameleonAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ChameleonApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
