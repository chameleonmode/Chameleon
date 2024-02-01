using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using System.IO;

namespace Chameleon.Infrastructure.Settings
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly string _settingsFilePath;

        public ApplicationSettingsService(
            IApplicationEnvironment applicationEnvironment
            )
        {
            _settingsFilePath = Path.Combine(
                applicationEnvironment.ApplicationDataFolderPath,
                "settings.json"
                );
        }

        private ApplicationSettings _settings;
        public IApplicationSettings Get()
        {
            if (_settings != null)
            {
                return _settings;
            }

            if (!File.Exists(_settingsFilePath))
            {
                _settings = new ApplicationSettings();
                return _settings;
            }

            var json = File.ReadAllText(_settingsFilePath);
            _settings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json);
            if (_settings == null)
            {
                _settings = new ApplicationSettings();
            }
            return _settings;
        }

        public void Save()
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_settings);
            File.WriteAllText(_settingsFilePath, json);
        }
    }
}
