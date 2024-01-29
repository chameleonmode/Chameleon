namespace Chameleon.Interfaces.YouTube
{
    public class YouTubeAddToOrderEventArgs : YouTubeBaseEventArgs
    {
        public IYouTubePublishVideoParameters YouTubePublishVideoParameters { get; set; }
    }
}
