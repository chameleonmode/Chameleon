using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.Views;
using Prism.Events;

namespace Chameleon.Application.Startup
{
    public class ApplicationStartup : IApplicationStartup
    {
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPopupDialogService _popupDialogService;
        private readonly IAuthService _authService;

        public ApplicationStartup(
            ISettingsService settingsService,
            IPopupDialogService popupDialogService,
            IEventAggregator eventAggregator,
            IAuthService authService)
        {
            _settingsService = settingsService;
            _popupDialogService = popupDialogService;
            _eventAggregator = eventAggregator;
            _authService = authService;
        }

        public async Task Run()
        {
            await _authService.Login();
            //if (string.IsNullOrEmpty(_settingsService.AuthAccessToken))
            //{
            // first create dialog not showing it

            //}
            // else
            // {
            //     _authManager.Login();
            // }
            //
            // return Task.CompletedTask;
        }

        private void ShowMainWindow()
        {
            //AutoUpdater.NET - check if newer version of app exists
            //AutoUpdater.Start("https://chameleonaccess.s3-us-west-2.amazonaws.com/AutoUpdater.xml");
        }

        private void CloseApplication()
        {
            Environment.Exit(0);
        }
    }
}
