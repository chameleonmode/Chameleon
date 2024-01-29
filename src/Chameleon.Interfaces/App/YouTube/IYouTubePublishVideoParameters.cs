using System;
using System.Threading;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubePublishVideoParameters 
        : IYouTubePublishBaseParameters
    {
        string Title { get; set; }
        string Description { get; set; }
        string PathToVideoFile { get; set; }
        string PathToThumbnailFile { get; set; }
        string[] Tags { get; set; }
        long SizeVideo { get; set; }
        Guid Id { get; set; }
        CancellationTokenSource CancellationToken { get; set; }
    }
}
