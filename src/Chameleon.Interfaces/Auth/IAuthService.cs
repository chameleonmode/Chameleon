using Chameleon.Interfaces.Ioc;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthService : ISingletonDependency
    {
        bool IsAuthenticated { get; }
        IAuthResult Login(string userName, string licenceKey);
        void Logout();
        Task<IAuthRefreshTokenResponse?> RefreshToken(string acessToken, string refreshToken, long delayInSeconds);

        bool IsLicenseActive(string license);

        //    /// <summary>
        //    /// Start auth process
        //    /// </summary>
        Task Login();
    }
}
