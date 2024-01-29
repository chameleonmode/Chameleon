using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Publishub
{
    public interface IPublishubViewManager : ISingletonDependency
    {
        IPublishubView GetOrCreateView(IUserProfile userProfile);
    }
}
