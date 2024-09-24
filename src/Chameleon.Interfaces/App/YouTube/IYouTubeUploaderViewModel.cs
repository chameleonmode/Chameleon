using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderViewModel : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
        bool IsInitialized { get; }
    }
}
