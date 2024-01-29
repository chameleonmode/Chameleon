using Chameleon.Interfaces.Proxies;

namespace Chameleon.Domain.Entities.Proxy
{
    public class ProxyAccess 
        : ProxySettings
        , IProxyAccess
    {
        public string Url { get; set; }
    }
}
