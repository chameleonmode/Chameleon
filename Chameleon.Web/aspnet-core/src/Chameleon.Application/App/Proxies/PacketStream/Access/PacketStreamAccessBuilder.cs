using Abp.Domain.Repositories;
using Chameleon.App.Dto;
using Chameleon.App.Entities;
using Chameleon.App.PacketStream;
using Chameleon.App.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class PacketStreamAccessBuilder : IPacketStreamAccessBuilder
    {
        private readonly IProxyConfiguration _proxyConfiguration;
        private readonly Random _random;
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public PacketStreamAccessBuilder(
            IProxyConfiguration proxyConfiguration
            )
        {
            _proxyConfiguration = proxyConfiguration;
            _random = new Random();
        }

        private static IList<ProxyCountryDto> _countries;

        public IList<ProxyCountryDto> GetCountries()
        {
            if (_countries == null)
            {
                _countries = _proxyConfiguration.Countries;
            }

            return _countries;
        }

        public IList<ProxyAccessDto> Build(
            ProxyAccessRequestDto input,
            ProxyCredit proxyCredit
            )
        {
            var countryParam = GetCountryUrlParam(input);
            var protocol = GetProtocol(input.ProtocolType);
            var host = GetHost(input.HostType);
            var port = GetPort(input.ProtocolType);
            var userName = GetUserName(proxyCredit);
            var password = GetPassword(proxyCredit);

            var result = new List<ProxyAccessDto>();
            for (var i = 0; i < input.Count; ++i)
            {
                var proxy = new ProxyAccessDto
                {
                    UserName = userName,
                    Password = password,
                    Host = host,
                    Port = port
                };

                var session = GetSessionUrlParam(input.IpType);
                var proxyPassword = $"{password}{countryParam}{session}";

                var proxyHost = $"{proxy.Host}:{port}";
                var proxyCredentials = $"{userName}:{proxyPassword}";

                proxy.Url = $"{protocol}{Uri.SchemeDelimiter}{proxyHost}:{proxyCredentials}";

                result.Add(proxy);
            }

            return result;
        }

        private string GetUserName(ProxyCredit proxyCredit)
        {
            if (IsMockData(proxyCredit))
            {
                return _proxyConfiguration.PacketStreamConfiguration.TestUserName;
            }
            return proxyCredit.ProxyUserName;
        }

        private string GetPassword(ProxyCredit proxyCredit)
        {
            if (IsMockData(proxyCredit))
            {
                return _proxyConfiguration.PacketStreamConfiguration.TestUserPassword;
            }
            return proxyCredit.ProxyAuthKey;
        }

        private bool IsMockData(ProxyCredit proxyCredit)
        {
            return proxyCredit.ProxyAuthKey == _proxyConfiguration.MockProxyAuthkey;
        }

        private string GetCountryUrlParam(ProxyAccessRequestDto input)
        {
            var countryId = input.CountryId;
            if (!countryId.HasValue || countryId.Value <=0)
            {
                return string.Empty;
            }

            var country = _countries.FirstOrDefault(x => x.Id == countryId);

            if (country is null)
            {
                return string.Empty;
            }

            var countryName = country.Name.Replace(" ", string.Empty);
            return $"_country-{countryName}";
        }

        private string GetHost(ProxyHostType hostType)
        {
            if (hostType == ProxyHostType.Hostname)
            {
                return "proxy.chameleonmode.com";
            }

            if (hostType == ProxyHostType.IpAddress)
            {
                return "3.228.244.201";
            }

            throw new NotImplementedException();
        }

        private int GetPort(ProxyProtocolType protocolType)
        {
            if (protocolType == ProxyProtocolType.Http)
            {
                return 31112;
            }

            if (protocolType == ProxyProtocolType.Ssl)
            {
                return 31111;
            }

            throw new NotImplementedException();
        }

        private string GetProtocol(ProxyProtocolType protocolType)
        {
            if (protocolType == ProxyProtocolType.Http)
            {
                return Uri.UriSchemeHttp;
            }

            if (protocolType == ProxyProtocolType.Ssl)
            {
                return Uri.UriSchemeHttps;
            }

            throw new NotImplementedException();
        }

        private string GetSessionUrlParam(ProxyIpType ipType)
        {
            if (ipType == ProxyIpType.Random)
            {
                return string.Empty;
            }

            if (ipType == ProxyIpType.Sticky)
            {
                return $"_session-{GetRandomString(8)}";
            }

            throw new NotImplementedException();
        }

        private string GetRandomString(int length)
        {
            var stringChars = new char[length];

            for (var i = 0; i < stringChars.Length; ++i)
            {
                stringChars[i] = Chars[_random.Next(Chars.Length)];
            }
            var finalString = new string(stringChars);
            return finalString;
        }
    }
}
