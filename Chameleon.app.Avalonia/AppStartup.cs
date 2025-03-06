using Avalonia.Media;

using Chameleon.app.Avalonia.lib.Community.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Auth;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Common.Util;
using Chameleon.lib.Helpers;

using FluentAvalonia.UI.Windowing;

using static Chameleon.lib.Common.Constants.Consts;

namespace Chameleon.app.Avalonia;
public class AppStartup {
	public event Action? OnLoginSuccess;

	public async Task RunAsync() {
		if (!await RunAsync(0)) {
			Toaster.Info("Login canceled, application closing");
			Environment.Exit(0);
		} else {
			try {
				IOtil.DeleteDir(Addons.AddonExtentionDir);
				IOtil.DeleteDir(Addons.CachedExtentionDir);
				await LoadSink();
				OnLoginSuccess?.Invoke();
			} catch (Exception ex) {
				_ = await Mbox.ShowErrorAsync("Invalid Login", "Browser authentication must match application email.\n" + ex.Message[ex.Message.LastIndexOf('\n')..]);
				await Session.Instance.Logout();
				await RunAsync();
			}
		}
	}
	public async Task<bool> RunAsync(int trys) {
		try {
			var loginSetings = IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) ?? new("", "", false);
			var loginvm = new MboxLoginViewModel {
				UserName = loginSetings.LoginName,
				LicenceKey = loginSetings.LicenseKey,
				AutoLogin = loginSetings.AutoLogin
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
			_ = await Session.Instance.Authenticate();
			Session.Instance.SetLogin(new LoginSettings(loginvm.UserName, loginvm.LicenceKey, loginvm.AutoLogin));
			//var loginDetailsChanged = loginvm.UserName != loginSetings.LoginName || loginvm.LicenceKey != loginSetings.LicenseKey || loginvm.AutoLogin != loginSetings.AutoLogin;

		} catch (Exception ex) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", ex.Message);
			if (trys < 1)
				return await RunAsync(trys++);
		}

		return Auther.AuthSession is not null;
	}

	public static async Task LoadSink(bool reload = false) {
		await DB.Instance.EnsureUser();
		var tasks = new List<Task>() {
			UserProfilesRepo.Instance.Load(),
			UserProfilesFolderRepo.Instance.Load(),
			TagsRepo.Instance.Load()
		};
		if (reload) {
			tasks.Add(UPAdditionalDataRepo.Instance.LoadReload(true));
		}
		await Task.WhenAll(tasks);
	}

	public static AppStartup Instance { get; } = new AppStartup();
	private AppStartup() {
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
			Toaster.Error("Error", e);
		};
		HttpApiClient.Instance.OnAuthError += async () => {
			try {
				await Auther.RefreshTokenAsync();
			} catch {
				Toaster.Error("AuthRefreshToken Err");
			}
		};
		HttpApiClient.Instance.OnCircuitBreaker += (e) => {
			//Toaster.ShowErr("CircuitBreaker", e);
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

	public class MainAppSplashScreen(object splashScreenContent) : IApplicationSplashScreen {
		public string? AppName { get; }
		public IImage? AppIcon { get; }
		public object SplashScreenContent { get; } = splashScreenContent;
		public int MinimumShowTime => 2000;

		public Func<Task>? InitApp { get; set; }

		public async Task RunTasks(CancellationToken cancellationToken) {
			if (InitApp != null)
				await InitApp.Invoke();
		}
	}
}