using Chameleon.Application.Events;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Startup;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Startup
{
    public class ApplicationStartup : IApplicationStartup
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAuthService _authService;

        public ApplicationStartup(
             IEventAggregator eventAggregator,
             IAuthService authService,
             IEnumerable<IApplicationEventHandlers> _)
        {
            _authService = authService;
            _eventAggregator = eventAggregator;

            _eventAggregator
                .GetEvent<LoginCancelEvent>()
                .SubscribeOnce(CloseApplication);
        }

        public async Task RunAsync()
        {
            if (!await RunAsync(0))
            {
                await MesageBoxHelper.ShowErrorAsync("Error Logging In", "There was an error validationg the login information that was provided.");
                CloseApplication();
            }
            else
                _eventAggregator
                       .GetEvent<LoginSuccessEvent>()
                       .Publish(new LoginEventArgs(null));
        }
        public async Task<bool> RunAsync(int trys)
        {
            bool success;
            try
            {
                success = await _authService.LoginAsync();
                if (!success)
                    success = await _authService.ShowLoginDialogAsync();
            }
            catch
            {
                if(trys < 1)
                    return await RunAsync(trys);

                success = false;
            }
            return success;
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
