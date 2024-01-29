using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthManager : ISingletonDependency
    {
        /// <summary>
        /// Start auth process
        /// </summary>
        void Login();

        /// <summary>
        /// Logout current user
        /// </summary>
        void Logout();
    }
}
