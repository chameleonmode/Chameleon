using Chameleon.Interfaces.Settings;

namespace Chameleon.Interfaces.Auth
{

    public interface ILimits
    {
        bool HasOutreach { get; }
        bool HasYouTube { get; }
        bool HasWordPress { get; }
        int MaxProfilesCount { get; }
        IContentDiscoveryLimits ContentDiscoveryLimits { get; }
        int MaxAssistantsCount { get; }
    }
}
