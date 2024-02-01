using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderView 
        : ITransientDependency
        , IUserProfileAccessor
    {
        bool IsInitialized { get; }
    }
}
