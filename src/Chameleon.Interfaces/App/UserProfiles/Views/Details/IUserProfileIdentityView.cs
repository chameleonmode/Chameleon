using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.UserProfiles
{
    public interface IUserProfileIdentityView
        : ITransientDependency
        , IUserProfileAccessor
    {
    }
}
