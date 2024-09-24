using Chameleon.Infrastructure.Api;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.OutReachLink
{
    public interface IProfileOutReachLinkApi
         : IApiLayer<
            ProfileOutReachLinkDto
            , int
            , CreateProfileOutReachLinkDto
            , ProfileOutReachLinkDto
            >
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
    }
}
