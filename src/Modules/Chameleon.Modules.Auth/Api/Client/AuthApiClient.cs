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

        public async Task<IAuthRefreshTokenResponse?> RefreshTokenAsync(string acessToken, string refreshToken)
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
