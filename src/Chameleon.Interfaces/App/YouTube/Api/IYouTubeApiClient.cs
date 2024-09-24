using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeApiClient : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        Task UnInitializeAsync();
        Task<IList<IYouTubeCategory>> GetVideoCategoriesAsync(string regionCode);
        Task<IList<IYouTubePlaylist>> GetPlayListsAsync();
        Task<bool> InitializeAsync(IUserProfile userProfile);
        Task InsertVideo(IYouTubePublishVideoParameters youTubeVideo);
        Task InsertThumbnail(IYouTubeThumbnail youTubeThumbnail);
        Task InsertVideoInPlayListAsync(string videoId, string playListId);
    }
}
