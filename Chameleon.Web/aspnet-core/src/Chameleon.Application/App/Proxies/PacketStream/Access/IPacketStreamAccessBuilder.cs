using Abp.Dependency;
using Chameleon.App.Dto;
using Chameleon.App.Entities;
using Chameleon.App.Shared.Proxies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IPacketStreamAccessBuilder : ITransientDependency
    {
        IList<ProxyCountryDto> GetCountries();

        IList<ProxyAccessDto> Build(
            ProxyAccessRequestDto input,
            ProxyCredit proxyCredit
            );
    }
}
