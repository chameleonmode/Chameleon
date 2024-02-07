namespace Chameleon.Interfaces.Auth
{
    public interface IContentDiscoveryLimits
    {
        bool HasProspector { get; }
        bool HasProspectorContent { get; }
        bool HasSocials { get; }
        bool HasSocialsContent { get; }
        int MaxRssCount { get; }
    }
}
