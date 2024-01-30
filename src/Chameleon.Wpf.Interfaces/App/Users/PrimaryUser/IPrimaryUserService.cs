using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Users.PrimaryUser
{
    public interface IPrimaryUserService
        : ISingletonDependency
    {
        void MarkGuidedTourDone();
    }
}
