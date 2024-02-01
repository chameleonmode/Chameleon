using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderViewManager : ISingletonDependency
    {
        IYouTubeUploaderView GetOrCreateView(IUserProfile userProfile);
        bool ViewIsInitialized(IUserProfile userProfile);
    }
}
