using System.Collections.Generic;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubePublishBaseParameters
    {
        IYouTubeCategory Category { get; set; }
        string License { get; set; }
        YouTubeVideoVisibility PrivacyStatus { get; set; }
        bool Embeddable { get; set; }
        bool PublicStatsViewable { get; set; }
        IList<IYouTubePlaylist> Playlists { get; set; }
    }
}
