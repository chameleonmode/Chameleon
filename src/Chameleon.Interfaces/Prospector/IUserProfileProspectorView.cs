using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Prospector
{
    public interface IUserProfileProspectorView
        : Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
        , IUserProfileAccessor
    {
    }
}
