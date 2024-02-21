using Chameleon.Configuration;
using Microsoft.Extensions.Configuration;
using System;

namespace Chameleon.App.PacketStream
{
    public class PacketStreamConfiguration : IPacketStreamConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PacketStreamConfiguration(
            IAppConfigurationAccessor configurationAccessor
            )
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        private const string AppConfigurationPrefix = "Proxy:PacketStream:";
        public string ApiHost => _appConfiguration[AppConfigurationPrefix + "ApiHost"];
        public string ApiAccessToken => _appConfiguration[AppConfigurationPrefix + "ApiAccessToken"];
        public string ApiEndpoint => $"https://{ApiHost}/reseller/";
        public string TestUserName => _appConfiguration[AppConfigurationPrefix + "TestUserName"];
        public string TestUserPassword => _appConfiguration[AppConfigurationPrefix + "TestUserPassword"];
        public int UserNameMaxLength => Int32.Parse(_appConfiguration[AppConfigurationPrefix + "UserNameMaxLength"]);// ;
    }
}
