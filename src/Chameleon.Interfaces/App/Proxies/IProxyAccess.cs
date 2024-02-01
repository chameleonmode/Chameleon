using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Proxies
{
    public interface IProxyAccess : IProxySettings
    {
        string Url { get; set; }
    }
}
