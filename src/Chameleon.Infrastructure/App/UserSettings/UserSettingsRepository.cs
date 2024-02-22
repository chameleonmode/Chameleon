using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserSettings;


namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsRepository
         : Repository<UserSetting,
            IUserSetting,
            UserSettingsDto
            >
        , IUserSettingsRepository
    {
        public UserSettingsRepository(
           IMapper mapper,
           IUserSettingsApi apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
        }
        protected override void OnSaved(IUserSetting entity)
        {
            base.OnSaved(entity);

            _eventAggregator
                .GetEvent<SavedUserSettingsEvent>()
                .Publish(new UserSettingsEventArgs(entity));
        }
    }
}
