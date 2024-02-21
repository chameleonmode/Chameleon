using Castle.Core.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chameleon.App.PacketStream
{
    public class PacketStreamGateway : IPacketStreamGateway
    {
        private readonly IPacketStreamConfiguration _packetStreamConfiguration;
        private static readonly Regex _userNameInvalidCharsRegex = new Regex("[^a-zA-Z0-9]");

        public PacketStreamGateway(
            IPacketStreamConfiguration packetStreamConfiguration
            )
        {
            _packetStreamConfiguration = packetStreamConfiguration;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { protected get; set; }
        public string ApiHost => _packetStreamConfiguration.ApiHost;
        public string ApiAccessToken => _packetStreamConfiguration.ApiAccessToken;
        public string ApiEndpoint => _packetStreamConfiguration.ApiEndpoint;

        public async Task<SubUserBalanceResponseData> CreateSubUserAsync(SubUserNameInputDto input)
        {
            CheckUserName(input);

            var response = await ExecuteSubUserRequestAsync<
                SubUserNameInputDto, SubUserBalanceResponse
                >("create", input);

            return response.Data;
        }

        public async Task<SubUserBalanceResponseData> GiveBalanceAsync(UpdateBalanceInputDto input)
        {
            CheckUserName(input);

            var response = await ExecuteSubUserRequestAsync<
                UpdateBalanceInputDto, SubUserBalanceResponse
                >("give_balance", input);

            return response.Data;
        }

        public async Task<SubUserBalanceResponseData> TakeBalanceAsync(UpdateBalanceInputDto input)
        {
            CheckUserName(input);

            var response = await ExecuteSubUserRequestAsync<
                UpdateBalanceInputDto, SubUserBalanceResponse
                >("take_balance", input);
            return response.Data;
        }

        public async Task<SubUserBalanceResponseData> ViewSubUserAsync(SubUserNameInputDto input)
        {
            CheckUserName(input);

            var response = await ExecuteSubUserRequestAsync<
                SubUserNameInputDto, SubUserBalanceResponse
                >("view_single", input);

            return response.Data;
        }

        private void CheckUserName(UserNameInputDto input)
        {
            RemoveInvalidCharsFromUserName(input);
            TrimUserNameByLength(input);
        }
        private void RemoveInvalidCharsFromUserName(UserNameInputDto input)
        {
            input.UserName = _userNameInvalidCharsRegex.Replace(input.UserName, "_").ToLower();
        }

        private void TrimUserNameByLength(UserNameInputDto input)
        {
            if (string.IsNullOrEmpty(input.UserName)) return;

            int maxLength = _packetStreamConfiguration.UserNameMaxLength;

            if (input.UserName.Length > maxLength)
            {
                input.UserName = input.UserName.Substring(0, maxLength);
            }

        }

        public async Task<SubUserBalanceResponseData> GetOrCreateSubUserAsync(SubUserNameInputDto input)
        {
            CheckUserName(input);

            try
            {
                return await ViewSubUserAsync(input);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "username invalid")
                {
                    return await CreateSubUserAsync(input);
                }

                throw;
            }
        }

        private Task<TResponse> ExecuteSubUserRequestAsync<TRequest, TResponse>(string endpoint, TRequest input)
            where TResponse : PacketStreamResponse
        {
            return ExecuteRequestAsync<TRequest, TResponse>($"sub_users/{endpoint}", input);
        }

        private async Task<TResponse> ExecuteRequestAsync<TRequest, TResponse>(string endpoint, TRequest input)
            where TResponse : PacketStreamResponse
        {
            var client = CreateClient(endpoint);
            var request = CreateRequest(input);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var result = JsonConvert.DeserializeObject<TResponse>(response.Content);

            HandleReponseMessage(result);
            if (result.IsSuccess)
            {
                return result;
            }

            throw new InvalidOperationException(result.Message);
        }

        private void HandleReponseMessage(PacketStreamResponse response)
        {
            var message = response.Message;
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.StartsWith("WARNING", StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.Warn(message);
            }
        }

        private RestClient CreateClient(string endpoint)
        {
            var client = new RestClient($"{ApiEndpoint}{endpoint}");
            client.Timeout = -1;
            return client;
        }

        private RestRequest CreateRequest()
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {ApiAccessToken}");
            return request;
        }

        private RestRequest CreateRequest(object body)
        {
            var request = CreateRequest();
            var requestBody = JsonConvert.SerializeObject(body);
            request.AddParameter("text/plain", requestBody, ParameterType.RequestBody);
            return request;
        }
    }
}
