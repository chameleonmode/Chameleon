using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderViewModel : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
        bool IsInitialized { get; }
    }
}
