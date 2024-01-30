using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.SocialNetwork
{
    public interface ISocialShareUrlService : ISingletonDependency
    {
        string GetShareUrl(SocialNetworkType type, string url);
    }
}