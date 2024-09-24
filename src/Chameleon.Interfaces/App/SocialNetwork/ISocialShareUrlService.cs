using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.SocialNetwork
{
    public interface ISocialShareUrlService : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string GetShareUrl(SocialNetworkType type, string url);
    }
}