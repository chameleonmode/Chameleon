using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Ioc;
using System.Net;
using System.Threading.Tasks;

namespace Chameleon.Auth.Api
{
    public interface IAuthApiClient : ISingletonDependency
    {
        Task<IAuthResponse> LoginAsync(NetworkCredential credentials);

        Task<IAuthRefreshTokenResponse?> RefreshTokenAsync(string acessToken, string refreshToken);

        Task<bool> IsLicenseActiveAsync(string license);
    }
}
