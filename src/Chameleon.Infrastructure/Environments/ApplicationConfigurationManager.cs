using Chameleon.Interfaces.Environments;
using System;
using System.Configuration;

namespace Chameleon.Infrastructure.Environments
{
    public class ApplicationConfigurationManager 
        : IApplicationConfigurationManager
    {
        private readonly Configuration _configuration;

        public ApplicationConfigurationManager()
        {
            _configuration = ConfigurationManager
                .OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string Get(string key, string defaultValue = "")
        {
            var value = _configuration.AppSettings.Settings[key];
            return value?.Value ?? defaultValue;
        }

        public T Get<T>(string key, T defaultValue = default(T))
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
            _configuration.AppSettings.Settings[key].Value 
                = value?.ToString().Trim() ?? string.Empty;

            if (save)
            {
                Save();
            }
        }

        public void Save()
        {
            _configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
