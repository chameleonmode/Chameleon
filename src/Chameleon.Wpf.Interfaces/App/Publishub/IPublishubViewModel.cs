using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Publishub
{
    public interface IPublishubViewModel : ITransientDependency
    {
        IUserProfile UserProfile { get; set; }
        bool IsInitialized { get; }
    }
}
