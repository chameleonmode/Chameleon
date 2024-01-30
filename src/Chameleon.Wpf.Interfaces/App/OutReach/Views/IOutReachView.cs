using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.OutReach
{
    public interface IOutReachView
        : ISingletonDependency
        , IUserProfileGetter
    {
        OutReachViewTab Tab { get; }

        void SetUserProfileLink(
            IUserProfile userProfile,
            OutReachViewTab tab);
    }
}
