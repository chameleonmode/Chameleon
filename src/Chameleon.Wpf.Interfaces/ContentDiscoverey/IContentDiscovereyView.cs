using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.ContentDiscoverey
{
    public interface IContentDiscovereyView
        : ISingletonDependency
        , IUserProfileAccessor
    {
    }
}
