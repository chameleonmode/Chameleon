using Chameleon.Interfaces.Proxies;

namespace Chameleon.Infrastructure.Proxies.Api.Dto
{
    public class ProxyCountryDto : IProxyCountry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
