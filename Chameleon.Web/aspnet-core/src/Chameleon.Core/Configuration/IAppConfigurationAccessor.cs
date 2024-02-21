using Microsoft.Extensions.Configuration;

namespace Chameleon.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
