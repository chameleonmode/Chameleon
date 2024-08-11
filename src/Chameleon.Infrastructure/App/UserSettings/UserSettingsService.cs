using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Settings;
using System;
using System.Linq;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsService
        : IUserSettingsService
    {
        public static UserSettingsService Instance => ContainerServiceHelper.Resolve<IUserSettingsService>() as UserSettingsService;
        private readonly IUserSettingsRepository _repository;

        public UserSettingsService(
            IUserSettingsRepository repository
            )
        {
            _repository = repository;
        }

       
        public IUserSetting Get()
        {
            var entity = _repository.GetAll().First();
            
            return entity;
        }

        public void Save(IUserSetting userSetting)
        {
            if (userSetting == null)
            {
                throw new ArgumentNullException();
            }

            if (userSetting.Id == 0)
            {
                _repository.Insert(userSetting);
            }
            else
            {
                _repository.Update(userSetting);
            }
        }
    }
}
