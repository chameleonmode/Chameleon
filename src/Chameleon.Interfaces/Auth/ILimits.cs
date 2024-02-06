using Chameleon.Auth.Api;

namespace Chameleon.Interfaces.Auth
{

    public interface ILimits
    {
        bool HasOutreach { get; }
        bool HasYouTube { get; }
        bool HasWordPress { get; }
        int MaxProfilesCount { get; }
        ContentDiscoveryLimits ContentDiscoveryLimits { get; }
        int MaxAssistantsCount { get; }
    }
}
