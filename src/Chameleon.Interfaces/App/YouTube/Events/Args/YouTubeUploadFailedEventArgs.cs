using System;

namespace Chameleon.Interfaces.YouTube
{
    public class YouTubeUploadFailedEventArgs : YouTubeBaseEventArgs
    {
        public Exception Exception { get; set; }
    }
}
