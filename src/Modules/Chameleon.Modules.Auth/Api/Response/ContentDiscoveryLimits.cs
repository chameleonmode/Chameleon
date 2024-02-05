using Chameleon.Interfaces.Auth;

namespace Chameleon.Auth.Api
{
    public class ContentDiscoveryLimits : IContentDiscoveryLimits
    {
        public bool HasProspector { get; set; }
        public bool HasProspectorContent { get; set; }
        public bool HasSocials { get; set; }
        public bool HasSocialsContent { get; set; }
        public int MaxRssCount { get; set; }
    }
}
