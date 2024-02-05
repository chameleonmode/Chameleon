using System.Net;
using System.Threading.Tasks;
using Chameleon.Auth.Api;
using Chameleon.Interfaces.Auth;

namespace Chameleon.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApiClient _apiClient;

        public AuthService(
            IAuthApiClient apiClient
            )
        {
            _apiClient = apiClient;
        }

        public bool IsAuthenticated { get; set; }

        public IAuthResult Login(string userName, string licenceKey)
        {
            var response =  _apiClient.Login(
                new NetworkCredential(userName, licenceKey)
                );

            IsAuthenticated = true;

            return new AuthResult
            {
                AuthToken = response.AccessToken,
                EncryptedAccessToken = response.EncryptedAccessToken,
                ExpireInSeconds = response.ExpireInSeconds,
                AuthRefreshToken = response.RefreshToken,
                UserId = response.UserId,
                CreatorUserId = response.CreatorUserId,
                UserName = userName,
                Permissions = response.Permissions,
                Limits = response.LicenseLimits,
                TookGuidedTour = response.TookGuidedTour, 
                CanCreateProfiles = response.CanCreateProfiles
            };
        }

        public async Task<IAuthRefreshTokenResponse> RefreshToken(string acessToken, string refreshToken, long delayInSeconds)
        {
            var response = await _apiClient.RefreshToken(acessToken, refreshToken, delayInSeconds);
            return response;
        }

        public void Logout()
        {
            IsAuthenticated = false;
        }

        public bool IsLicenseActive(string license) 
        {
            return _apiClient.IsLicenseActive(license);
        }
    }
}
