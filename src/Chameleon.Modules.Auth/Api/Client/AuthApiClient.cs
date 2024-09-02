using Chameleon.Auth.Api.Client;
using Chameleon.Auth.Api.Response;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using System.Net;

namespace Chameleon.Auth.Api
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly IApiClient _apiClient;

        public static IAuthApiClient Instance => ContainerServiceHelper.Resolve<IAuthApiClient>();

        public AuthApiClient(
            IApiClient apiClient
            )
        {
            _apiClient = apiClient;
        }

        public async Task<IAuthResponse> LoginAsync(NetworkCredential credentials)
        {
            var requestDto = new AuthRequestDto
            {
                UserNameOrEmailAddress = credentials.UserName,
                Password = credentials.Password
            };

            var response = await _apiClient.PostAsync<AuthResponse>("TokenAuth/Authenticate", requestDto);

            if (!response.IsValid)
            {
                throw new WebException("Response do not contain token");
            }
            return response;
        }

        public async Task<IAuthRefreshTokenResponse?> RefreshTokenAsync(string acessToken, string refreshToken, long delayInSeconds)
        {

            //await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));

            var requestDto = new RefreshTokenRequestDto
            {
                AccessToken = acessToken,
                RefreshToken = refreshToken,
            };

            AuthRefreshTokenResponse? response = null;

            try
            {
                response = await _apiClient.PostAsync<AuthRefreshTokenResponse>("TokenAuth/RefreshToken", requestDto);
            }
            catch { }

            return response;
        }

        public async Task<bool> IsLicenseActiveAsync(string license) 
        {
            var response = await _apiClient.GetAsync<IsLicActiveResponseDTO>($"TokenAuth/IsLicenseActive?key={license}");
            return response.isActive;
        }
    }
}
