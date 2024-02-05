using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Views;
using Chameleon.Core.Extensions;
using Prism.Services.Dialogs;
using System.Threading.Tasks;

namespace Chameleon.Auth.Core
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthSession _authSession;
        private readonly IAuthService _authService;
        private readonly IDialogManager _dialogManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationUser _applicationUser;

        public AuthManager(
            IAuthSession authUser,
            IAuthService authService,
            IDialogManager dialogManager,
            IEventAggregator eventAggregator,
            IApplicationUser applicationUser
            )
        {
            _authSession = authUser;
            _authService = authService;
            _dialogManager = dialogManager;
            _eventAggregator = eventAggregator;
            _applicationUser = applicationUser;
        }

        public void Login()
        {
            // first create dialog not showing it
            var dialog = _dialogManager.Create(typeof(IAuthView), result =>
            {
                // check for not success result
                if (result.Result != ButtonResult.OK)
                {
                    // reject auth
                    OnAuthenticateCancel();
                    return;
                }

                // get view model from result
                var viewModel = result.Parameters
                    .GetValue<IAuthViewModel>(nameof(IViewModel));

                // call success
                OnAuthenticateSuccess(viewModel);
            });

            // show
            dialog.ShowDialog();
        }

        public void Logout()
        {
            _authService.Logout();

            _eventAggregator
                .GetEvent<LogoutEvent>()
                .Publish(new EventArgs());
        }

        private async void OnAuthenticateSuccess(IAuthViewModel viewModel)
        {
            // get login result
            var loginResult = viewModel.AuthResult;

            // trigger event
            OnAuthenticateSuccess(loginResult);
            await RefreshToken(loginResult.AuthToken, loginResult.AuthRefreshToken, loginResult.ExpireInSeconds);
        }

        private void OnAuthenticateSuccess(IAuthResult authResult)
        {
            // store credential to current user
            _authSession.UserId = authResult.UserId;
            _authSession.CreatorUserId = authResult.CreatorUserId;
            _authSession.UserName = authResult.UserName;
            _authSession.AuthToken = authResult.AuthToken;
            _authSession.ExpireInSeconds = authResult.ExpireInSeconds;
            _authSession.EncryptedAccessToken = authResult.EncryptedAccessToken;
            _authSession.AuthRefreshToken = authResult.AuthRefreshToken;
            _authSession.Permissions = authResult.Permissions;
            _authSession.Limits = authResult.Limits;
            _authSession.TookGuidedTour = authResult.TookGuidedTour;
            _authSession.CanCreateProfiles = authResult.CanCreateProfiles;

            // trigger event
            _eventAggregator
                .GetEvent<LoginSuccessEvent>()
                .Publish(new LoginEventArgs(_authSession));
        }

        private void OnAuthenticateCancel()
        {
            _eventAggregator
                .GetEvent<LoginCancelEvent>()
                .Publish(new EventArgs());
        }

        private async Task RefreshToken(string acessToken, string refreshToken, long delayInSeconds)
        {
            var response = await _authService.RefreshToken(acessToken, refreshToken, delayInSeconds);

            if (response == null)
            {
                Login();
            }
            else
            {
                _authSession.AuthToken = response.NewAccessToken;
                _authSession.AuthRefreshToken = response.NewRefreshToken;
                _authSession.ExpireInSeconds = response.ExpireInSeconds;

                await RefreshToken(_authSession.AuthToken, _authSession.AuthRefreshToken, _authSession.ExpireInSeconds);
            }
        }
    }
}
