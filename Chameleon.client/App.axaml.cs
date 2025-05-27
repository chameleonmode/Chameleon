using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Styling;

using Chameleon.app.Avalonia;
using Chameleon.app.Avalonia.ViewModels;
using Chameleon.app.Avalonia.Views;
using Chameleon.lib;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Interfaces.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chameleon.lib.Const;
using Chameleon.lib.Util;
using Chameleon.client.Features.ProfilesAndFolders.Profiles.Identity;
using Chameleon.client.Features;
using Chameleon.lib.Helpers;
using Chameleon.lib.Common.ServiceManagers;
using Chameleon.lib.Auth;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.app.Avalonia.lib.Community.Controls;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Api;
using Chameleon.lib.Api.Repos;
using Chameleon.lib.Abs.Platformatic;
using Chameleon.client.Services;
using Chameleon.client.UI.UserControls;
using FluentAvalonia.UI.Windowing;
using Avalonia.VisualTree;

namespace Chameleon.client;

public class AppStartup {
	public event Action? OnLoginSuccess;

	public async Task RunAsync() {
		if (!await RunAsync(0)) {
			Toaster.Info("Login canceled, application closing");
			Environment.Exit(0);
		} else {
			try {
				Toaster.Success($"Hello {(Session.Instance.Login?.LoginName) ?? "World"}");
				await LoadSink();
				OnLoginSuccess?.Invoke();
			} catch (Exception ex) {
				_ = await Mbox.ShowErrorAsync("Invalid Login", "Browser authentication must match application email.\n" + (ex.Message.Contains('\n') ? ex.Message[ex.Message.LastIndexOf('\n')..] : ex.Message));
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
				await Mbox.ShowTaskDialog<MboxLoginUserControl, MboxLoginViewModel>(new(
					() => loginvm,
					"User Login",
					"Enter the provided activation information",
					Symbas: Enums.Symbas.ContactInfo,
					Btns: Enums.MBoxButtons.OkCancel
				)) == Enums.TaskDialogResult.Cancel) {
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
			tasks.Add(UPAdditionalDataRepo.Instance.Load());
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
}

public partial class App : Application {
	public static Window? GetMainWindow => (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
	public static IStorageProvider StorageProvider {
		get {
			return (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.StorageProvider
			?? throw new InvalidOperationException("StorageProvider not available");
		}
	}


	public static T? TryGetResource<T>(string key) where T : class {
		return Current?.TryGetResource(key, null, out var result) == true && result is T typed ? typed : default;
	}

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);

		// Default logic doesn't auto detect windows theme anymore in designer
		// force here
		if (Design.IsDesignMode) {
			RequestedThemeVariant = ThemeVariant.Light;
		}

		// IoC.Instance.StartUps.Add(AddonsServer.Instance);

		IoC.Instance.Configure(() => {
			return new WritableConfiguration(new ConfigurationBuilder()
				.SetBasePath(FilePaths.AppDataDir)
				.AddJsonFile(Variables.AppSettingsFileName, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build(), Path.Combine(FilePaths.AppDataDir, Variables.AppSettingsFileName));
		}, (services) => {
			_ = services
			.AddSingleton<IDispatchService, DispatchService>()
			.AddSingleton<IToasterService, ToasterService>()
			.AddSingleton<IMboxService, MboxService>()
			.AddSingleton<IShowWindowService, ShowWindowService>()
			.AddSingleton<ICopyPastaService, CopyPastaService>()
			//FunctionalSettings
			.AddSingleton<FunctionalSettingsView>()
			.AddSingleton<UserProxySettingsView>()
			.AddSingleton<UserDefaultSettingsView>()
			.AddSingleton<PhoneVerificationView>()
			.AddSingleton<ProxyCreditView>()
			.AddSingleton<FunctionalSettingsViewModel>()
			.AddSingleton<UserProxySettingsViewModel>()
			.AddSingleton<UserDefaultSettingsViewModel>()
			.AddSingleton<PhoneVerificationViewModel>()
			.AddSingleton<ProxyCreditViewModel>()
			.WithAllPagesAndFeatures();
		});

		// Setup IoC
		IoC.Instance.Init(action: async (inited) => {
			if (inited) {
				await AppStartup.Instance.RunAsync();
				IoC.GetService<Features.Settings.ViewModel>()?.InitializSettings();

				_ = await lib.WebBrowser.Project.Init();
				_ = await lib.Playwright.Project.Init();
			}
		});

		Navigator.Instance.RegisterView(nameof(IdentityView), typeof(IdentityView));
		Navigator.Instance.RegisterView(nameof(FunctionalSettingsView), typeof(FunctionalSettingsView));
	}

	public override void OnFrameworkInitializationCompleted() {
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			var window = new UI.Windows.Main {
				DataContext = ViewModel.Instance,
				SplashScreen = new SplashScreen(),
				// ExtendClientAreaToDecorationsHint = true;
				// ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.Default;
			};
			window.TitleBar.ExtendsContentIntoTitleBar = true;
			window.TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
#if DEBUG
			window.AttachDevTools();
			window.Topmost = true;
#endif

			/** TODO: test if still happening on windows then remove fully 
			Dictionary<Control, object> TooltipBackup = [];
			window.Deactivated += (s, e) => {
				var controlsWithTooltips = new List<Control>();
				var queue = new Queue<Visual>();
				queue.Enqueue(window);

				while (queue.Count > 0) {
					var current = queue.Dequeue();

					if (current is Control control && ToolTip.GetTip(control) != null) controlsWithTooltips.Add(control);

					foreach (var child in current.GetVisualChildren()) {
						queue.Enqueue(child);
					}
				}
				foreach (var control in controlsWithTooltips) {
					var tooltip = ToolTip.GetTip(control);
					if (tooltip != null) {
						TooltipBackup[control] = tooltip;
						ToolTip.SetTip(control, null);
					}
				}
			};
			window.Activated += (s, e) => {
				var controlsToRestore = TooltipBackup.Keys.ToList();
				foreach (var control in controlsToRestore) {
					if (TooltipBackup.TryGetValue(control, out var tooltip)) {
						ToolTip.SetTip(control, tooltip);
						TooltipBackup.Remove(control);
					}
				}
			};
			**/
			desktop.MainWindow = window;
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new View {
				DataContext = ViewModel.Instance
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}
