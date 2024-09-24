using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderViewManager : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IYouTubeUploaderView GetOrCreateView(IUserProfile userProfile);
        bool ViewIsInitialized(IUserProfile userProfile);
    }
}
