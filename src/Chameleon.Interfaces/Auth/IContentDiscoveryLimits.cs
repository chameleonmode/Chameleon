using Chameleon.Auth.Api;
using System.Text.Json.Serialization;

namespace Chameleon.Interfaces.Auth
{

    [JsonDerivedType(typeof(ContentDiscoveryLimits), "ContentDiscoveryLimits")]
    public interface IContentDiscoveryLimits
    {
        bool HasProspector { get; }
        bool HasProspectorContent { get; }
        bool HasSocials { get; }
        bool HasSocialsContent { get; }
        int MaxRssCount { get; }
    }
}
