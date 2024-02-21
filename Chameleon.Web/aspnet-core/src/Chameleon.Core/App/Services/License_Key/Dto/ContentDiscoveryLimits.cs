namespace Chameleon.App.Services.License_Key.Dto
{
    public class ContentDiscoveryLimits
    {
        public bool HasProspector { get; set; }
        public bool HasProspectorContent { get; set; }
        public bool HasSocials { get; set; }
        public bool HasSocialsContent { get; set; }
        public int MaxRssCount { get; set; }

        public ContentDiscoveryLimits(
            bool hasProspector = false,
            bool hasProspectorContent = false,
            bool hasSocials = false,
            bool hasSocialsContent = false,
            int maxRssCount = 0)
        {
            HasProspector = hasProspector;
            HasProspectorContent = hasProspectorContent;
            HasSocials = hasSocials;
            HasSocialsContent = hasSocialsContent;
            MaxRssCount = maxRssCount;
        }
    }
}
