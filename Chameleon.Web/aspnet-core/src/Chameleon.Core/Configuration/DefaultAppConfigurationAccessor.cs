using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Chameleon.Configuration
{
    /* This service is replaced in Web layer and Test project separately */
    public class DefaultAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }
        
        public DefaultAppConfigurationAccessor(IWebHostEnvironment environment)
        {
            Configuration = environment.GetAppConfiguration();
        }
    }
}