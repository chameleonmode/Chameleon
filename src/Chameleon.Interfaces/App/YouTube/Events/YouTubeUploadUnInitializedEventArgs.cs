using System;

namespace Chameleon.Interfaces.YouTube
{
    public class YouTubeUploadUnInitializedEventArgs 
        : EventArgs 
    { 
        public int ProfileId { get; set; }
        public YouTubeUploadUnInitializedEventArgs(int profileId)
        {
            ProfileId = profileId;
        }
    }
}
