using Chameleon.Interfaces.Ioc;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Auth
{
    public interface IAuthService : ISingletonDependency
    {
        bool IsAuthenticated { get; }
        void Logout();
        Task RefreshTokenAsync(string acessToken, string refreshToken, long delayInSeconds);

        Task<bool> IsLicenseActive(string license);

        //    /// <summary>
        //    /// Start auth process
        //    /// </summary>
        Task<bool> LoginAsync();
        Task<bool> ShowLoginDialogAsync();
        void Login();
    }
}
