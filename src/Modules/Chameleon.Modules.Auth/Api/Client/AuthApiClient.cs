using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Chameleon.Auth.Api.Client;
using Chameleon.Auth.Api.Response;
using Chameleon.Interfaces.Api;
using Chameleon.Interfaces.Auth;

namespace Chameleon.Auth.Api
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly IApiClient _apiClient;

        public AuthApiClient(
            IApiClient apiClient
            )
        {
            _apiClient = apiClient;
        }

        public IAuthResponse Login(NetworkCredential credentials)
        {
            var requestDto = new AuthRequestDto
            {
                UserNameOrEmailAddress = credentials.UserName,
                Password = credentials.Password
            };

            var response = _apiClient.Post<AuthResponse>("TokenAuth/Authenticate", requestDto);

            if (!response.IsValid)
            {
                throw new WebException("Response do not contain token");
            }
            return response;
        }

        public async Task<IAuthRefreshTokenResponse> RefreshToken(string acessToken, string refreshToken, long delayInSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));

            var requestDto = new RefreshTokenRequestDto
            {
                AccessToken = acessToken,
                RefreshToken = refreshToken
            };

            AuthRefreshTokenResponse response = null;

            try
            {
                response = _apiClient.Post<AuthRefreshTokenResponse>("TokenAuth/RefreshToken", requestDto);
            }
            catch { }

            return response;
        }

        public bool IsLicenseActive(string license) 
        {
            var response = _apiClient.Get<IsLicActiveResponseDTO>($"TokenAuth/IsLicenseActive?key={license}");
            return response.isActive;
        }
    }
}
