using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsService
        : IUserDefaultSettingsService
    {
        private readonly IUserDefaultSettingsRepository _repository;

        public UserDefaultSettingsService(
            IUserDefaultSettingsRepository repository
            )
        {
            _repository = repository;
        }

        private IUserDefaultSettings _settings;
        private IUserDefaultSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    var entities = _repository.GetAll();
                    _settings = new UserDefaultSettings()
                    {
                        entities
                    };
                }
                return _settings;
            }
        }

        public IUserDefaultSettings GetAll()
        {
            return Settings;
        }

        public IUserDefaultSetting Create()
        {
            var entity = new UserDefaultSetting();

            var userDefaultSettings = Settings;
            userDefaultSettings.Add(entity);
            return entity;
        }

        public IUserDefaultSetting Get(int Id)
        {
            var entity = _repository.Get(Id);
            return entity;
        }

        public void Save(IUserDefaultSetting userDefaultSettings)
        {
            if (userDefaultSettings == null)
            {
                throw new ArgumentNullException();
            }

            if (userDefaultSettings.Id == 0)
            {
                _repository.Insert(userDefaultSettings);
            }
            else
            {
                _repository.Update(userDefaultSettings);
            }
        }

        public void Delete(IUserDefaultSetting userDefaultSettings)
        {
            if (userDefaultSettings == null)
            {
                throw new ArgumentNullException();
            }

            if (userDefaultSettings.Id != 0)
            {
                _repository.Delete(userDefaultSettings.Id);
            }
            Settings.Remove(userDefaultSettings);
        }


        public Task<string> GetRandomUrlAsync() =>
            Task.Run(() =>
            {
                var all = GetAll();
                if (all?.Any() == true)
                {
                    var random = new Random();
                    return all[random.Next(all.Count)].DefaultUrl;
                }

                return "about:blank";
            });
    }
}
