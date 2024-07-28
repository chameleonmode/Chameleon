using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Settings
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly string _settingsFilePath;
        private readonly SemaphoreSlim l = new SemaphoreSlim(1, 1);  

        private ApplicationSettings _settings;

        public static IApplicationSettingsService Instance { get; } = ContainerServiceHelper.Resolve<IApplicationSettingsService>() as ApplicationSettingsService;

        public ApplicationSettingsService(
            IApplicationEnvironment applicationEnvironment
            )
        {
            _settingsFilePath = Path.Combine(
                applicationEnvironment.ApplicationDataFolderPath,
                "settings.json"
                );
        }


        public async Task Save()
        {
            await l.WaitAsync();
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(_settings);
                await Task.Run(() => File.WriteAllText(_settingsFilePath, json));
            }
            finally
            {
                l.Release();
            }
        }

        public async Task<IApplicationSettings> GetAsync()
        {
            //await l.WaitAsync();
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

            //l.Release();
            return _settings;
        }

        public async Task Logout()
        {
            _settings.Settings.AutoLogin = false;
            await Save();
            Environment.Exit(0);
        }
    }
}
