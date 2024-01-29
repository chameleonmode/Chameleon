namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeThumbnail
    {
        string PathToThumbnailFile { get; set; }
        string VideoId { get; set; }
        string MimeType { get; set; }
    }
}
