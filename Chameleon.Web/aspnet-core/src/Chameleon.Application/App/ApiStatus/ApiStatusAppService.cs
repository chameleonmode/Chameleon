using Chameleon.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class ApiStatusAppService
        : ChameleonAppServiceBase
        , IApiStatusAppService
    {
        private const string ConfigurationKeyPrefix = "TestLoginUser:";

        private readonly StringContent _authRequestContent;
        private readonly IConfigurationRoot _configuration;
        private readonly IApiStatusManager _apiStatusManager;
        private readonly string _url;

        public ApiStatusAppService(
            IAppConfigurationAccessor configurationAccessor
            , IApiStatusManager apiStatusManager
            )
        {
            _configuration = configurationAccessor.Configuration;
            _apiStatusManager = apiStatusManager;
            _url = $"{_configuration["App:ServerRootAddress"]}api/TokenAuth/Authenticate";

            var authRequest = new AuthRequestDto
            {
                UserNameOrEmailAddress = _configuration[ConfigurationKeyPrefix + "UserNameOrEmailAddress"],
                Password = _configuration[ConfigurationKeyPrefix + "Password"]
            };

            _authRequestContent = new StringContent(
                JsonConvert.SerializeObject(authRequest),
                Encoding.UTF8,
                "application/json"
                );

            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public async Task GetStatusAsync()
        {
            if (!_apiStatusManager.IsOld)
            {
                ThrowLoginExeption(_apiStatusManager.LoginIsFailed);
                return;
            }

            await GetStatusLoginAsync();
        }

        private async Task GetStatusLoginAsync()
        {
            _apiStatusManager.LoginIsFailed = true;

            var loginResult = await SendLoginRequestAsync();

            ThrowLoginExeption(!loginResult.Success || !loginResult.Result.IsValid);

            _apiStatusManager.LoginIsFailed = false;
        }

        private void ThrowLoginExeption(bool isFailed)
        {
            if (isFailed)
            {
                throw new WebException("Test login is failed");
            }
        }

        private async Task<AuthResponseResultDto> SendLoginRequestAsync()
        {
            using (var client = new HttpClient())
            {
                var result = await client.PostAsync(_url, _authRequestContent);
                var resultString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthResponseResultDto>(resultString);
            }
        }
    }
}
