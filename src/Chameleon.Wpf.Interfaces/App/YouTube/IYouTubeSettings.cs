namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeSettings
    {
        string ApiKey { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }
}
