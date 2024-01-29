using Chameleon.Interfaces.YouTube;

namespace Chameleon.Domain.Entities
{
    public class YoutubeSettings : IYouTubeSettings
    {
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
