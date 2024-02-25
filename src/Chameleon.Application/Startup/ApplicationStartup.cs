using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Startup;
using Chameleon.Interfaces.Views;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Startup
{
    public class ApplicationStartup : IApplicationStartup
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAuthService _authService;

        public ApplicationStartup(
             IEventAggregator eventAggregator,
            IAuthService authService)
        {
            _authService = authService;
             _eventAggregator = eventAggregator;

            _eventAggregator
                .GetEvent<LoginCancelEvent>()
                .SubscribeOnce(CloseApplication);

            _eventAggregator
              .GetEvent<LoginSuccessEvent>()
              .SubscribeOnce(ShowMainWindow);
        }

        public async Task RunAsync()
        {
            await _authService.LoginAsync();
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

        public void Run()
        {
            _authService.Login();
        }
    }
}
