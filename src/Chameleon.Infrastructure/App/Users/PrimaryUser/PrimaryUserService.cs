using Chameleon.Interfaces.App.Users.PrimaryUser;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser
{
    public class PrimaryUserService 
        : IPrimaryUserService
    {
        private readonly IPrimaryUserRepository _primaryUserRepository;

        public PrimaryUserService(
            IPrimaryUserRepository primaryUserRepository
            )
        {
            _primaryUserRepository = primaryUserRepository;
        }
        public void MarkGuidedTourDone()
        {
            _primaryUserRepository.MarkGuidedTourDone();
        }
    }
}
