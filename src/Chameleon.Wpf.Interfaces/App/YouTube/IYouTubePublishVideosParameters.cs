namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubePublishVideosParameters
        : IYouTubePublishBaseParameters
    {
        string TitleTemplate { get; set; }
        string DescriptionTemplate { get; set; }
        string PathToVideoFiles { get; set; }
        string PathToThumbnailFiles { get; set; }
        string Tags { get; set; }
    }
}
