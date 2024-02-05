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
        private readonly IApplicationConfigurationManagerService applicationConfigService;
        public ApplicationConfigurationManager(IApplicationConfigurationManagerService applicationConfigService)
        {
            //_configuration = ConfigurationManager
            //    .OpenExeConfiguration(ConfigurationUserLevel.None);
            this.applicationConfigService = applicationConfigService;
        }

        public string Get(string key, string defaultValue = "")
        {
            //var value = _configuration.AppSettings.Settings[key];
            //return value?.Value ?? defaultValue;
            return applicationConfigService.Get(key, defaultValue);
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            //var value = Get(key);
            //if (string.IsNullOrWhiteSpace(value))
            //{
            //    return defaultValue;
            //}
            //return (T)Convert.ChangeType(value, typeof(T));
            return applicationConfigService.Get<T>(key, defaultValue);
        }

        public void Set(string key, object value, bool save = true)
        {
            //_configuration.AppSettings.Settings[key].Value 
            //    = value?.ToString().Trim() ?? string.Empty;
            applicationConfigService.Set(key, value, save);
            if (save)
            {
                Save();
            }
        }

        public void Save()
        {
            applicationConfigService.Save();
            //_configuration.Save(ConfigurationSaveMode.Full, true);
            //ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
