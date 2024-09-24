using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.YouTube
{
    public interface IYouTubeUploaderView 
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
        , IUserProfileAccessor
    {
        bool IsInitialized { get; }
    }
}
