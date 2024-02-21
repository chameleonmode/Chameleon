using Abp.Dependency;
using Chameleon.App.Dto;
using Chameleon.App.PacketStream;
using System.Collections.Generic;

namespace Chameleon.App
{
    public interface IProxyConfiguration
        : ISingletonDependency
    {
        IPacketStreamConfiguration PacketStreamConfiguration { get; }
        IList<ProxyCountryDto> Countries { get; }
        string MockProxyAuthkey { get; }
    }
}
