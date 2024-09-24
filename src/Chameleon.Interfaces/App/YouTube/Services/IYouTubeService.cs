using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeService 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        Task<bool> InitializeAsync(IUserProfile userProfile);
        Task<IList<IYouTubePlaylist>> GetPlayListsAsync();
        Task<IList<IYouTubeCategory>> GetVideoCategoriesAsync();
        void PublishVideos(IYouTubePublishVideosParameters parameters);
        Task UnInitializeAsync();
    }
}
