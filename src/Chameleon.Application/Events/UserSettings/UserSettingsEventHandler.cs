using Chameleon.Core.Extensions;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.Settings;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserSettings;
using Chameleon.Interfaces.Windows;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Events
{
    public class UserSettingsEventHandler 
        : IUserSettingsEventHandler
    {
        private readonly IMainWindow _mainWindow;
        private readonly ISettingsView _settingsView;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserDefaultSettingsService _userDefaultSettingsService;

        public UserSettingsEventHandler(
            IMainWindow mainWindow,
            ISettingsView settingsView,
            IEventAggregator eventAggregator,
            IUserDefaultSettingsService userDefaultSettingsService)
        {
            _mainWindow = mainWindow;
            _settingsView = settingsView;
            _eventAggregator = eventAggregator;
            _userDefaultSettingsService = userDefaultSettingsService;

            _eventAggregator
              .GetEvent<DeleteUserDefaultSettingsEvent>()
              .Subscribe(args => DeleteDefaultSettings(args.UserDefaultSetting));

            _eventAggregator
               .GetEvent<CreateUserDefaultSettingsEvent>()
               .Subscribe(CreateDefaultSettings);

            _eventAggregator
                .GetEvent<OpenChangeProxiesEvent>()
                .Subscribe(args => OpenChangeProxies(args.FolderId));            
        }

        private void OpenChangeProxies(int folderId)
        {
            _mainWindow.ShowWaitIndicator();
            this.InvokeOnUiThread(() =>
            {
                _settingsView.SetTabContent(SettingTabs.ProxySettings);
                _mainWindow.SetContent(_settingsView, "Settings");
                _mainWindow.HideWaitIndicator();

                _eventAggregator
                    .GetEvent<UserProxySetFolderIdEvent>()
                    .Publish(new UserProxySetFolderIdEventArgs(folderId));
            });
        }

        private void CreateDefaultSettings()
        {
            _userDefaultSettingsService.Create();
        }

        private void DeleteDefaultSettings(IUserDefaultSetting userDefaultSettings)
        {
            _userDefaultSettingsService.Delete(userDefaultSettings);
        }
    }
}
