using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeMessageService
        : ISingletonDependency
    {
        void PathToVideoIsError();
        void PathToThumbnailIsError();
        void NumberFilesMismatch();
        void VideoFolderIsEmpty(string path);
        void ThumbnailFolderIsEmpty(string path);
        void VideoFileIsTooBig();
        void ThumbnailFileIsTooBig();
        void DailyLimit();
        void YouTubeSettingsIsEmpty();
        bool UserSecretChanged();
        void YoutubeUploadLimit();
        void GenericYouTubeErrorMessage(int code, string message);
    }
}
