using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;
using Prism.Events;

namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsRepository
         : Repository<UserDefaultSetting,
            IUserDefaultSetting,
            int,
            UserDefaultSettingsDto,
            CreateUserDefaultSettingsDto,
            UserDefaultSettingsDto,
            GetAllRequestDto
            >
        , IUserDefaultSettingsRepository
    {
        public UserDefaultSettingsRepository(
           IMapper mapper,
           IUserDefaultSettingsApi apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
        }
        protected override void OnSaved(IUserDefaultSetting entity)
        {
            base.OnSaved(entity);

            _eventAggregator
                .GetEvent<SavedUserDefaultSettingsEvent>()
                .Publish(new UserDefaultSettingsEventArgs(entity));
        }
    }
}
