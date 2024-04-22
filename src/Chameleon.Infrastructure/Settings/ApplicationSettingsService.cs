using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Settings;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        //public IApplicationSettings Get()
        //{
        //    if (_settings != null)
        //    {
        //        return _settings;
        //    }

        //    if (!File.Exists(_settingsFilePath))
        //    {
        //        _settings = new ApplicationSettings();
        //        return _settings;
        //    }

        //    var json = File.ReadAllText(_settingsFilePath);
        //    _settings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json);
        //    if (_settings == null)
        //    {
        //        _settings = new ApplicationSettings();
        //    }
        //    return _settings;
        //}
        static SemaphoreSlim l = new SemaphoreSlim(1, 1);
        public async Task Save()
        {
            await l.WaitAsync();
            string json = System.Text.Json.JsonSerializer.Serialize(_settings);
            await Task.Run(() => File.WriteAllText(_settingsFilePath, json));
            l.Release();
        }

        public async Task<IApplicationSettings> GetAsync()
        {
            await l.WaitAsync();
            if (_settings != null)
            {
                return _settings;
            }

            if (!File.Exists(_settingsFilePath))
            {
                _settings = new ApplicationSettings();
                return _settings;
            }

            var json = await Task.Run(()=> File.ReadAllText(_settingsFilePath));
            _settings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json);
            if (_settings == null)
            {
                _settings = new ApplicationSettings();
            }

            l.Release();
            return _settings;
        }
    }
}
