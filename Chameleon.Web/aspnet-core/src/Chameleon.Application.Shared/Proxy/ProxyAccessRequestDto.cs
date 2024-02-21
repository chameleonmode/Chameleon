namespace Chameleon.App.Shared.Proxies
{
    public class ProxyAccessRequestDto
    {
        public ProxyIpType IpType { get; set; }
        public ProxyProtocolType ProtocolType { get; set; }
        public ProxyHostType HostType { get; set; }
        public int? CountryId { get; set; }
        public int Count { get; set; }
    }
}
