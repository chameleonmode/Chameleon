using System.Net;
using System.Threading.Tasks;
using Chameleon.Auth.Api;
using Chameleon.Auth.Api.Response;
using Chameleon.Common.Helpers;
using Chameleon.Core.Extensions;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Settings;
using Chameleon.Prism.Events;

namespace Chameleon.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationUser _applicationUser;
        private readonly IAuthSession _authSession;
        private readonly IAuthApiClient _apiClient;
        private readonly IAuthTaskDialogViewModel _authContentDialogService;  
        private readonly IApplicationSettingsService _settingsService;

        private IApplicationSettings _appSettings;
        private System.Timers.Timer _pollingTimer;

        public AuthService(IAuthApiClient apiClient,
            IAuthSession authSession,
            IEventAggregator eventAggregator,
            IApplicationSettingsService settingsService,
            IApplicationUser applicationUser,
            IAuthTaskDialogViewModel contentDialogService)
        {
            _settingsService = settingsService;
            //_appSettings = settingsService.Get();
            _authSession = authSession;
            _applicationUser = applicationUser;
            _eventAggregator = eventAggregator;
            _apiClient = apiClient;
            _authContentDialogService = contentDialogService;
        }

        public bool IsAuthenticated { get; set; }

        public async Task<bool> LoginAsync()
        {
            IAuthResult? loginResult = null;
            try
            {
                _appSettings = await _settingsService.GetAsync();
                if(!_appSettings.Settings.AutoLogin)
                    return false;

                if (_appSettings.Login.LoginName.HasAny() && _appSettings.Login.LicenseKey.HasAny())
                {
                    loginResult = await Login(_appSettings.Login.LoginName, _appSettings.Login.LicenseKey);
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
                    _pollingTimer = new System.Timers.Timer(TimeSpan.FromSeconds(loginResult.ExpireInSeconds));
                    _pollingTimer.Elapsed += async (s, e) =>
                    {
                        await RefreshTokenAsync(loginResult.AuthToken, loginResult.AuthRefreshToken, loginResult.ExpireInSeconds);
                    };
                    _pollingTimer.Start();

                    //_ = RefreshTokenAsync(loginResult.AuthToken, loginResult.AuthRefreshToken, loginResult.ExpireInSeconds);
                }
            }
            //if (loginResult is null || refreshTokenResponse is null)
            //    _eventAggregator.GetEvent<LoginFailEvent>().Publish();

            return loginResult is not null && loginResult.HasAuthToken;
        }
        public async Task<bool> ShowLoginDialogAsync()
        {
            var result = await _authContentDialogService.ShowAsync();

            if (result == IContentDialogResult.Primary)
            {
                return await LoginAsync();
            }
            else
            {
               return false;
            }
        }
        
        public  void Login()
        {
           // _popupDialogService.ShowDialogInWindow<IAuthLoginView, IWindowWindowDialog>("login", async (ResultNum) =>
           // {
           //     if (ResultNum is not null && (PopupDialogButtonResult)ResultNum is PopupDialogButtonResult r)
           //         switch (r)
           //         {
           //             //case PopupDialogButtonResult.Cancel:
           //             //case PopupDialogButtonResult.Unset:
           //             //case PopupDialogButtonResult.None:
           //             default:
           //                 //_eventAggregator.GetEvent<LoginCancelEvent>().Publish();
           //                 break;
           //             case PopupDialogButtonResult.OK:
           //                 await LoginAsync();
           //                 return;
           //         }
           //
           //     _eventAggregator.GetEvent<LoginCancelEvent>().Publish();
           // });
        }

        public async Task<IAuthResult> Login(string userName, string licenceKey)
        {
            var response = await _apiClient.LoginAsync(
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

        }


        public async Task RefreshTokenAsync(string acessToken, string refreshToken, long delayInSeconds)
        {
            var response = await _apiClient.RefreshTokenAsync(acessToken, refreshToken, delayInSeconds);
            if (response == null)
            {
                bool relogin = await ShowLoginDialogAsync();

                if (!relogin)
                {
                    _eventAggregator.GetEvent<LoginCancelEvent>().Publish();
                }

                return;
            }

            _authSession.AuthToken = response.NewAccessToken;
            _authSession.AuthRefreshToken = response.NewRefreshToken;
            _authSession.ExpireInSeconds = response.ExpireInSeconds;

            await RefreshTokenAsync(_authSession.AuthToken, _authSession.AuthRefreshToken, _authSession.ExpireInSeconds);
        }

        public void Logout()
        {
            IsAuthenticated = false;

            _eventAggregator
                .GetEvent<LogoutEvent>()
                .Publish();
        }

        public async Task<bool> IsLicenseActive(string license) 
        {
            return await _apiClient.IsLicenseActiveAsync(license);
        }

    }
}
