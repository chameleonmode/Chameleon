using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Users.PrimaryUser
{
    public interface IPrimaryUserService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void MarkGuidedTourDone();
    }
}
