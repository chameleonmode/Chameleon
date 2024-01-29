using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Prospector
{
    public interface IUserProfileProspectorView
        : ITransientDependency
        , IUserProfileAccessor
    {
    }
}
