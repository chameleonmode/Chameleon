using Chameleon.app.Avalonia.lib.Community.Controls;
using Chameleon.app.Avalonia.lib.Community.ViewModels;
using Chameleon.app.Avalonia.Models;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.lib.Api;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.app.Avalonia;
public class AppStartup {
	public event Action? OnLoginSuccess;

	private readonly IAuthSession? _authSession;

	public async Task RunAsync()
	{
		if (!await RunAsync(0)) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", "There was an error validationg the login information that was provided.");
			Environment.Exit(0);
		} else {
			OnLoginSuccess?.Invoke();
		}
	}
	public async Task<bool> RunAsync(int trys)
	{
		var success = false;
	  var loginvm = new MboxLoginViewModel();
		try {
			if (IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) is LoginSettings login) {
				try {
					loginvm.UserName = login.LoginName;
					loginvm.LicenceKey = login.LicenseKey;
					success = await Login(loginvm.UserName, loginvm.LicenceKey);
				} catch {
					Toaster.ShowErr("Error Logging In", "There was an error validationg the login information that was provided.");
				}
			}
			if (!success) {
				var res = await Mbox.ShowTaskDialog<MboxLoginViewModel, MboxLoginUserControl>(() => loginvm, "User Login", "Enter the provided activation information", symbas: Enums.Symbas.ContactInfo, btns: Enums.MBoxButtons.OkCancel);
				if (res == Enums.TaskDialogResult.Cancel)
					return success;
				ArgumentNullException.ThrowIfNull(loginvm.UserName, "UserName");
				ArgumentNullException.ThrowIfNull(loginvm.LicenceKey, "LicenceKey");

				success = await Login(loginvm.UserName, loginvm.LicenceKey);
				if (success) {
					IoC.SetJsonValue(new LoginSettings(loginvm.UserName, loginvm.LicenceKey, true), nameof(LoginSettings));
					return success;
				}
			}
		} catch (Exception ex) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", ex.Message);
			if (trys < 1)
				return await RunAsync(trys++);
		}

		return success;
	}

	[Obsolete("Added for compatibility with corrent infrastructure project until _authSession refactoed out only")]
	private async Task<bool> Login(string user, string pass)
	{
		var authResult = await Auther.LoginAsync(user, pass);
		if (authResult is not null && _authSession is not null) {
			_authSession.UserName = user;
			_authSession.AuthToken = authResult.AccessToken!;
			_authSession.AuthRefreshToken = authResult.AccessToken!;
			_authSession.UserId = authResult.UserId;
			_authSession.CreatorUserId = authResult.CreatorUserId;
			_authSession.ExpireInSeconds = authResult.ExpireInSeconds;
			_authSession.EncryptedAccessToken = authResult.EncryptedAccessToken ?? string.Empty;
			_authSession.Permissions = authResult.Permissions;
			_authSession.Limits = new TheseLimits() {
				HasOutreach = authResult.LicenseLimits.HasOutreach,
				HasYouTube = authResult.LicenseLimits.HasYouTube,
				HasWordPress = authResult.LicenseLimits.HasWordPress,
				MaxProfilesCount = authResult.LicenseLimits.MaxProfilesCount,
				MaxAssistantsCount = authResult.LicenseLimits.MaxAssistantsCount,
				ContentDiscoveryLimits = new TheseContentDiscoveryLimits() {
					HasProspector = authResult.LicenseLimits.ContentDiscoveryLimits.HasProspector,
					HasProspectorContent = authResult.LicenseLimits.ContentDiscoveryLimits.HasProspectorContent,
					HasSocials = authResult.LicenseLimits.ContentDiscoveryLimits.HasSocials,
					HasSocialsContent = authResult.LicenseLimits.ContentDiscoveryLimits.HasSocialsContent,
					MaxRssCount = authResult.LicenseLimits.ContentDiscoveryLimits.MaxRssCount
				},
			};
			_authSession.TookGuidedTour = authResult.TookGuidedTour;
			_authSession.CanCreateProfiles = authResult.CanCreateProfiles;
			return true;
		}
		return false;
	}

	public static AppStartup Instance { get; } = new AppStartup();
	private AppStartup()
	{
		_authSession = ContainerServiceHelper.Resolve<IAuthSession>();
		HttpApiClient.Instance.OnRetry += (e) => {
				Toaster.ShowErr("Error", e);
		};
		HttpApiClient.Instance.OnAuthError += async() => {
			try {
				if (_authSession is not null) {
					var acessToken = _authSession.AuthToken;
					var refreshToken = _authSession.AuthRefreshToken;
					var delayInSeconds = _authSession.ExpireInSeconds;
					var response = await Auther.RefreshTokenAsync(acessToken, refreshToken);
					_authSession.AuthToken = response.NewAccessToken!;
					_authSession.AuthRefreshToken = response.NewRefreshToken!;
					_authSession.ExpireInSeconds = response.ExpireInSeconds;
				}
			} catch {
				Toaster.ShowErr("AuthRefreshToken Err");
			}
		};
		HttpApiClient.Instance.OnCircuitBreaker += (e) => {
			Toaster.ShowErr("CircuitBreaker", e);
		};
	}
}

public class TheseLimits : ILimits {
	public bool HasOutreach { get; set; }
	public bool HasYouTube { get; set; }
	public bool HasWordPress { get; set; }
	public int MaxProfilesCount { get; set; }
	public int MaxAssistantsCount { get; set; }
	public TheseContentDiscoveryLimits ContentDiscoveryLimits { get; set; } = new TheseContentDiscoveryLimits();
	IContentDiscoveryLimits ILimits.ContentDiscoveryLimits => ContentDiscoveryLimits;
}
public class TheseContentDiscoveryLimits : IContentDiscoveryLimits {
	public bool HasProspector { get; set; }
	public bool HasProspectorContent { get; set; }
	public bool HasSocials { get; set; }
	public bool HasSocialsContent { get; set; }
	public int MaxRssCount { get; set; }
}
