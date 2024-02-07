using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Settings;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Chameleon.Auth.Api
{
    public class Limits : ILimits
    {
        public bool HasOutreach { get; set; }
        public bool HasYouTube { get; set; }
        public bool HasWordPress { get; set; }
        public int MaxProfilesCount { get; set; }
        public ContentDiscoveryLimits ContentDiscoveryLimits { get; set; } = new ContentDiscoveryLimits();
        IContentDiscoveryLimits ILimits.ContentDiscoveryLimits => ContentDiscoveryLimits;
        public int MaxAssistantsCount { get; set; }
    }
}
