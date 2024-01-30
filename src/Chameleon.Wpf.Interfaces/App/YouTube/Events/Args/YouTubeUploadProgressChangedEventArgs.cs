namespace Chameleon.Interfaces.YouTube
{
    public class YouTubeUploadProgressChangedEventArgs 
        : YouTubeBaseEventArgs
    {
        public long BytesSent { get; set; }
    }
}
