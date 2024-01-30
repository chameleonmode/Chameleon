using Chameleon.Interfaces.Repository;

namespace Chameleon.Interfaces.App.Users.PrimaryUser
{
    public interface IPrimaryUserRepository
        : IRepository<IPrimaryUser>
    {
        void MarkGuidedTourDone();
    }
}
