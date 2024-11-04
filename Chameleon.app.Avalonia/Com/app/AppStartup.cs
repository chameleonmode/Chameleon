using Chameleon.app.Avalonia.lib.Community.Controls;
using Chameleon.app.Avalonia.Models;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Sys;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;

namespace Chameleon.app.Avalonia;
public class AppStartup {
	public event Action? OnLoginSuccess;

	public async Task RunAsync()
	{
		if (!await RunAsync(0)) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", "There was an error validationg the login information that was provided.");
			Environment.Exit(0);
		} else {
			await IOtil.DC(Consts.AppTempDir);
			await LoadSink();
			OnLoginSuccess?.Invoke();
		}
	}
	public async Task<bool> RunAsync(int trys)
	{
		try {
			var loginSetings = IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) ?? new("", "", false);
			var loginvm = new MboxLoginViewModel {
				UserName = loginSetings.LoginName,
				LicenceKey = loginSetings.LicenseKey,
				AutoLogin = true
			};

			if (!loginSetings.AutoLogin && 
				await Mbox.ShowTaskDialog<MboxLoginViewModel, MboxLoginUserControl>(() => loginvm, 
					"User Login", 
					"Enter the provided activation information", 
					symbas: Enums.Symbas.ContactInfo, 
					btns: Enums.MBoxButtons.OkCancel) == Enums.TaskDialogResult.Cancel) {
				return false;
			}
			ArgumentNullException.ThrowIfNull(loginvm.UserName, "UserName");
			ArgumentNullException.ThrowIfNull(loginvm.LicenceKey, "LicenceKey");

			await Auther.LoginAsync(loginvm.UserName, loginvm.LicenceKey);
			if (Auther.AuthSession is not null &&
				(loginvm.UserName   != loginSetings.LoginName || 
				 loginvm.LicenceKey != loginSetings.LicenseKey ||
				 loginvm.AutoLogin != loginSetings.AutoLogin)) {
				IoC.SetJsonValue(new LoginSettings(loginvm.UserName, loginvm.LicenceKey, loginvm.AutoLogin), nameof(LoginSettings));
				return true;
			}
		} catch (Exception ex) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", ex.Message);
			if (trys < 1)
				return await RunAsync(trys++);
		}

		return Auther.AuthSession is not null;
	}

	public static Task LoadSink(bool reload = false)
	{
		var tasks = new List<Task>() {
			UserProfilesRepo.Instance.Load(),
			UserProfilesFolderRepo.Instance.Load()
		};
		if (reload) {
			tasks.Add(UPAdditionalDataRepo.Instance.LoadReload(true));
		}
		return Task.WhenAll(tasks);
	}

	public static AppStartup Instance { get; } = new AppStartup();
	private AppStartup()
	{
		// for migration
		//if (IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) is null || IoC.GetJsonValue<AppSettings>(nameof(AppSettings)) is null) {
		//	var _settingsFilePath = Path.Combine(
		//			Consts.AppDataLocalDir,
		//			"settings.json"
		//			);
		//	if (File.Exists(_settingsFilePath)) {
		//		var json = File.ReadAllText(_settingsFilePath);
		//		var _settings = System.Text.Json.JsonSerializer.Deserialize<TheseApplicationSettings>(json);
		//		if (_settings is not null) {
		//			if (_settings.Login is not null) {
		//				IoC.SetJsonValue(new LoginSettings(_settings.Login.LoginName, _settings.Login.LicenseKey, true),
		//					nameof(LoginSettings));
		//			}

		//			if (_settings.Settings is not null) {
		//				IoC.SetJsonValue(new AppSettings(_settings.Settings.CurrentAppTheme, _settings.Settings.CustomAccentColor?.ToString(), _settings.Settings.UseCustomAccentColor),
		//					nameof(AppSettings));
		//			}
		//		}
		//	}
		//}
		//_authSession = IoC.GetService<IAuthSession>();
		HttpApiClient.Instance.OnRetry += (e) => {
			Toaster.ShowErr("Error", e);
		};
		HttpApiClient.Instance.OnAuthError += async () => {
			try {
				await Auther.RefreshTokenAsync();
			} catch {
				Toaster.ShowErr("AuthRefreshToken Err");
			}
		};
		HttpApiClient.Instance.OnCircuitBreaker += (e) => {
			Toaster.ShowErr("CircuitBreaker", e);
		};
		HttpApiClient.Instance.OnSendSeccess += (m) => {
			//switch(m) {
			//	case HttpMethod.Get:
			//		Toaster.ShowInfo($"Request {m} was successful.");
			//		break;
			//	case HttpMethod.Post:
			//		Toaster.ShowSuccess($"Request {m} was successful.");
			//		break;
			//	case HttpMethod.Put:
			//		Toaster.ShowSuccess($"Request {m} was successful.");
			//		break;
			//	case HttpMethod.Delete:
			//		Toaster.ShowSuccess($"Request {m} was successful.");
			//		break;
			//}
			//if(m == HttpMethod.Put)
			//	Toaster.ShowSuccess($"Update was successful.");
		};
	}
}
//	[Obsolete("Added for compatibility with corrent infrastructure project until _authSession refactoed out only")]
//	private async Task<bool> Login(string user, string pass)
//	{
//		var authResult = await Auther.LoginAsync(user, pass);
//		if (authResult is not null && _authSession is not null) {
//			_authSession.UserName = user;
//			_authSession.AuthToken = authResult.AccessToken!;
//			_authSession.AuthRefreshToken = authResult.AccessToken!;
//			_authSession.UserId = authResult.UserId;
//			_authSession.CreatorUserId = authResult.CreatorUserId;
//			_authSession.ExpireInSeconds = authResult.ExpireInSeconds;
//			_authSession.EncryptedAccessToken = authResult.EncryptedAccessToken ?? string.Empty;
//			_authSession.Permissions = authResult.Permissions;
//			_authSession.Limits = new TheseLimits() {
//				HasOutreach = authResult.LicenseLimits.HasOutreach,
//				HasYouTube = authResult.LicenseLimits.HasYouTube,
//				HasWordPress = authResult.LicenseLimits.HasWordPress,
//				MaxProfilesCount = authResult.LicenseLimits.MaxProfilesCount,
//				MaxAssistantsCount = authResult.LicenseLimits.MaxAssistantsCount,
//				ContentDiscoveryLimits = new TheseContentDiscoveryLimits() {
//					HasProspector = authResult.LicenseLimits.ContentDiscoveryLimits.HasProspector,
//					HasProspectorContent = authResult.LicenseLimits.ContentDiscoveryLimits.HasProspectorContent,
//					HasSocials = authResult.LicenseLimits.ContentDiscoveryLimits.HasSocials,
//					HasSocialsContent = authResult.LicenseLimits.ContentDiscoveryLimits.HasSocialsContent,
//					MaxRssCount = authResult.LicenseLimits.ContentDiscoveryLimits.MaxRssCount
//				},
//			};
//			_authSession.TookGuidedTour = authResult.TookGuidedTour;
//			_authSession.CanCreateProfiles = authResult.CanCreateProfiles;
//			return true;
//		}
//		return false;
//	}
//}

//public class TheseLimits : ILimits {
//	public bool HasOutreach { get; set; }
//	public bool HasYouTube { get; set; }
//	public bool HasWordPress { get; set; }
//	public int MaxProfilesCount { get; set; }
//	public int MaxAssistantsCount { get; set; }
//	public TheseContentDiscoveryLimits ContentDiscoveryLimits { get; set; } = new TheseContentDiscoveryLimits();
//	IContentDiscoveryLimits ILimits.ContentDiscoveryLimits => ContentDiscoveryLimits;
//}
//public class TheseContentDiscoveryLimits : IContentDiscoveryLimits {
//	public bool HasProspector { get; set; }
//	public bool HasProspectorContent { get; set; }
//	public bool HasSocials { get; set; }
//	public bool HasSocialsContent { get; set; }
//	public int MaxRssCount { get; set; }
//}
//public class TheseApplicationSettings  {
//	public TheseLoginSettings Login { get; set; } = new TheseLoginSettings();
//	public TheseSettingsSettings Settings { get; set; } = new TheseSettingsSettings();
//}
//public class TheseSettingsSettings {
//	public string CurrentAppTheme { get; set; } = "System";
//	public string? CustomAccentColor { get; set; }
//	public bool UseCustomAccentColor { get; set; }
//	public bool AutoLogin { get; set; } = true;
//	public string? CodesverifyApiKey { get; set; }
//	public string? UserScriptsDirectory { get; set; }
//	public string? SMSPoolApiKey { get; set; }
//}
//public class TheseLoginSettings {
//	public string LoginName { get; set; } = string.Empty;
//	public string LicenseKey { get; set; } = string.Empty;

//	public void Set(string loginName, string licenseKey)
//	{
//		LoginName = loginName ?? string.Empty;
//		LicenseKey = licenseKey ?? string.Empty;
//	}
//}

//public class ThisAuthSession : IAuthSession {
//	public long UserId { get; set; }
//	public long? CreatorUserId { get; set; }
//	public string? UserName { get; set; }
//	public string? AuthToken { get; set; }
//	public bool HasAuthToken => !string.IsNullOrEmpty(AuthToken);
//	public long ExpireInSeconds { get; set; }
//	public string? EncryptedAccessToken { get; set; }
//	public string? AuthRefreshToken { get; set; }
//	public string[]? Permissions { get; set; }
//	public ILimits? Limits { get; set; }
//	public bool TookGuidedTour { get; set; }
//	public bool CanCreateProfiles { get; set; }
//}
