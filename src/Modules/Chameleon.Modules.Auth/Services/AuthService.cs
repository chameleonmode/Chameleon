using System.Net;
using System.Threading.Tasks;
using Chameleon.Auth.Api;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;

namespace Chameleon.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPopupDialogService _popupDialogService;
        private readonly IApplicationSettings _appSettings;
        private readonly IApplicationUser _applicationUser;
        private readonly IAuthSession _authSession;
        private readonly IAuthApiClient _apiClient;

        public AuthService(IAuthApiClient apiClient,
            IAuthSession authSession,
            IEventAggregator eventAggregator,
            IPopupDialogService popupDialogService,
            IApplicationSettingsService settingsService,
            IApplicationUser applicationUser)
        {
            _appSettings = settingsService.Get();
            _authSession = authSession;
            _applicationUser = applicationUser;
            _eventAggregator = eventAggregator;
            _apiClient = apiClient;
            _popupDialogService = popupDialogService;
        }

        public bool IsAuthenticated { get; set; }

        public async Task LoginAsync()
        {
            IAuthResult? loginResult = null;
            try
            {
                if (_appSettings.Login.LoginName.HasAny() && _appSettings.Login.LicenseKey.HasAny())
                {
                    await Task.Run(() =>
                    {
                        loginResult = Login(_appSettings.Login.LoginName, _appSettings.Login.LicenseKey);
                    });
                }
            }
            catch
            {
                loginResult = null;
            }
            finally
            {
                if (loginResult is not null)
                {
                    OnAuthenticateSuccess(loginResult);
                    await RefreshToken(loginResult.AuthToken, loginResult.AuthRefreshToken, loginResult.ExpireInSeconds);
                }
                else
                {
                    _eventAggregator.GetEvent<LoginFailEvent>().Publish();
                }
            }
        }

        public void Login()
        {
            _popupDialogService.ShowDialog("AuthView", "login", async (ResultNum) =>
            {
                switch ((PopupDialogButtonResult)ResultNum)
                {
                    case PopupDialogButtonResult.Cancel:
                    case PopupDialogButtonResult.Unset:
                    case PopupDialogButtonResult.None:
                    default:
                        _eventAggregator.GetEvent<LoginCancelEvent>().Publish();
                        break;
                    case PopupDialogButtonResult.OK:
                        await LoginAsync();
                        break;
                }
            });
        }

        public IAuthResult Login(string userName, string licenceKey)
        {
            var response =  _apiClient.Login(
                new NetworkCredential(userName, licenceKey)
                );

            IsAuthenticated = true;

            return new AuthResult
            {
                AuthToken = response.AccessToken,
                EncryptedAccessToken = response.EncryptedAccessToken,
                ExpireInSeconds = response.ExpireInSeconds,
                AuthRefreshToken = response.RefreshToken,
                UserId = response.UserId,
                CreatorUserId = response.CreatorUserId,
                UserName = userName,
                Permissions = response.Permissions,
                Limits = response.LicenseLimits,
                TookGuidedTour = response.TookGuidedTour, 
                CanCreateProfiles = response.CanCreateProfiles
            };
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


        public async Task<IAuthRefreshTokenResponse?> RefreshToken(string acessToken, string refreshToken, long delayInSeconds)
        {
            var response = await _apiClient.RefreshToken(acessToken, refreshToken, delayInSeconds);
            if (response == null)
            {
                await LoginAsync();
                //return null;
            }
            else
            {
                _authSession.AuthToken = response.NewAccessToken;
                _authSession.AuthRefreshToken = response.NewRefreshToken;
                _authSession.ExpireInSeconds = response.ExpireInSeconds;

               // return await RefreshToken(_authSession.AuthToken, _authSession.AuthRefreshToken, _authSession.ExpireInSeconds);
            }
            return response;
        }

        public void Logout()
        {
            IsAuthenticated = false;

            _eventAggregator
                .GetEvent<LogoutEvent>()
                .Publish();
        }

        public bool IsLicenseActive(string license) 
        {
            return _apiClient.IsLicenseActive(license);
        }
    }
}
