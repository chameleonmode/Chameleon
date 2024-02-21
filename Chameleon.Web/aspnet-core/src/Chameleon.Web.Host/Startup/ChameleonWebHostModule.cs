using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Chameleon.Configuration;

namespace Chameleon.Web.Host.Startup
{
    [DependsOn(
       typeof(ChameleonWebCoreModule))]
    public class ChameleonWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ChameleonWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ChameleonWebHostModule).GetAssembly());
        }
    }
}
