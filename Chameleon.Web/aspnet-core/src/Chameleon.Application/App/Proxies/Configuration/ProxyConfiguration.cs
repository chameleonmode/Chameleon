using Chameleon.App.Dto;
using Chameleon.App.PacketStream;
using Chameleon.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Chameleon.App
{
    public class ProxyConfiguration
        : IProxyConfiguration
    {
        private const string AppConfigurationPrefix = "Proxy:";
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IPacketStreamConfiguration _packetStreamConfiguration;
        private readonly IList<ProxyCountryDto> _countries;

        public ProxyConfiguration(
            IAppConfigurationAccessor configurationAccessor
            , IPacketStreamConfiguration packetStreamConfiguration
            )
        {
            _appConfiguration = configurationAccessor.Configuration;
            _packetStreamConfiguration = packetStreamConfiguration;

            var countriesSection = _appConfiguration.GetSection(AppConfigurationPrefix + "Countries");
            _countries = countriesSection.Get<IList<ProxyCountryDto>>();
        }

        public IPacketStreamConfiguration PacketStreamConfiguration => _packetStreamConfiguration;

        public IList<ProxyCountryDto> Countries => _countries;
        public string MockProxyAuthkey => _appConfiguration[AppConfigurationPrefix + "MockProxyAuthkey"];
    }
}
