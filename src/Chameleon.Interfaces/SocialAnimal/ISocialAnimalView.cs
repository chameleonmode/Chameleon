using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.SocialAnimal
{
    public interface ISocialAnimalView
        : ISingletonDependency
        , IUserProfileAccessor
    {
    }
}
