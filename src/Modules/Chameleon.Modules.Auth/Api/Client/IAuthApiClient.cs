using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Ioc;
using System.Net;
using System.Threading.Tasks;

namespace Chameleon.Auth.Api
{
    public interface IAuthApiClient : ISingletonDependency
    {
        IAuthResponse Login(NetworkCredential credentials);

        Task<IAuthRefreshTokenResponse> RefreshToken(string acessToken, string refreshToken, long delayInSeconds);

        bool IsLicenseActive(string license);
    }
}
