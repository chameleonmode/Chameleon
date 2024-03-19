using Chameleon.Core.Settings;
using Chameleon.Interfaces.Environment;
using Chameleon.Interfaces.Environments;
using System;
using System.Configuration;

namespace Chameleon.Infrastructure.Environments
{
    public class ApplicationConfigurationManager 
        : IApplicationConfigurationManager
    {
        //private readonly Configuration _configuration;
        //private readonly IApplicationConfigurationManagerService applicationConfigService;
        public ApplicationConfigurationManager()
        {
            //_configuration = ConfigurationManager
            //    .OpenExeConfiguration(ConfigurationUserLevel.None);
           // this.applicationConfigService = applicationConfigService;
        }

        public string Get(string key, string defaultValue = "") => key switch
        {
            "apiBaseUrl" => GlobalSettings.ApiBaseUrl,
            "notionProfile" => GlobalSettings.NotionProfile,
            "notionUrl" => GlobalSettings.NotionUrl,
            "apiSocialAnimalUrl" => GlobalSettings.ApiSocialAnimalUrl,
            "websiteUrl" => GlobalSettings.WebsiteUrl,
            "supportUrl" => GlobalSettings.SupportUrl,
            "facebookGroupUrl" => GlobalSettings.FacebookGroupUrl,
            "pricingUrl" => GlobalSettings.PricingUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(key), $"Not expected value: {key}"),
        };

        public T Get<T>(string key, T defaultValue = default)
        {
            var value = Get(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void Set(string key, object value, bool save = true)
        {
            throw new NotImplementedException();
            // Preferences.Set(key, value?.ToString()?.Trim() ?? string.Empty);
        }

        public void Save()
        {

        }
    }
}
